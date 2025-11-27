using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml; // Añadido por EPPlus

// Asegúrate de que este sea el namespace correcto
namespace Proyecto01.API.Controllers 
{
    [ApiController]
    [Route("api/carga")]
    public class CargaController : ControllerBase
    {
        // Modelo para la respuesta (equivalente al CargaResponseDto de Android)
        public class CargaResponse
        {
            public int TotalRecords { get; set; }
            public int Compliant { get; set; }
            public int NonCompliant { get; set; }
            public List<SlaRecordResponse> SlaRecords { get; set; } = new();
        }

        // Modelo para cada registro (equivalente al SlaRecordDto de Android)
        public class SlaRecordResponse
        {
            public string Id { get; set; }
            public string Codigo { get; set; }
            public string Rol { get; set; }
            public string FechaSolicitud { get; set; }
            public string FechaIngreso { get; set; }
            public string TipoSla { get; set; }
            public int DiasSla { get; set; }
            public bool Cumple { get; set; }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No se ha enviado ningún archivo válido." });
            }

            // Usamos EPPlus dentro de un using para asegurar que se liberen los recursos
            using var package = new ExcelPackage(file.OpenReadStream());

            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                return BadRequest(new { message = "El archivo Excel no contiene ninguna hoja de cálculo." });
            }

            var response = new CargaResponse();
            var rowCount = worksheet.Dimension.Rows;

            // Empezamos desde la fila 2 para saltarnos el encabezado
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // Leemos los datos de cada celda
                    var rol = worksheet.Cells[row, 1].GetValue<string>()?.Trim();
                    var fechaSolicitud = worksheet.Cells[row, 2].GetValue<DateTime>();
                    var fechaIngreso = worksheet.Cells[row, 3].GetValue<DateTime>();
                    var tipoSla = worksheet.Cells[row, 4].GetValue<string>()?.Trim();
                    var codigo = worksheet.Cells[row, 5].GetValue<string>()?.Trim() ?? $"GEN-{row}";

                    // Validación básica
                    if (string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(tipoSla)) continue;

                    var diasSla = (int)(fechaIngreso - fechaSolicitud).TotalDays;
                    var cumple = tipoSla.ToUpper() == "SLA1" ? diasSla < 35 : diasSla < 20;

                    response.SlaRecords.Add(new SlaRecordResponse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Codigo = codigo,
                        Rol = rol,
                        FechaSolicitud = fechaSolicitud.ToString("dd/MM/yyyy"),
                        FechaIngreso = fechaIngreso.ToString("dd/MM/yyyy"),
                        TipoSla = tipoSla,
                        DiasSla = diasSla,
                        Cumple = cumple
                    });
                }
                catch (Exception ex)
                {
                    // Si una fila tiene un formato incorrecto, la ignoramos y continuamos
                    Console.WriteLine($"Error procesando fila {row}: {ex.Message}");
                    continue;
                }
            }

            // Calculamos los totales
            response.TotalRecords = response.SlaRecords.Count;
            response.Compliant = response.SlaRecords.Count(r => r.Cumple);
            response.NonCompliant = response.SlaRecords.Count(r => !r.Cumple);

            // TODO: Aquí iría la lógica para guardar response.SlaRecords en la base de datos

            return Ok(response);
        }
    }
}