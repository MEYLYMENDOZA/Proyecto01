using Proyecto01.CORE.Core.DTOs;
using Proyecto01.CORE.Core.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Proyecto01.CORE.Core.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        // 1. Método GetAll (Restaurado)
        public async Task<IEnumerable<ReporteListDTO>> GetAll()
        {
            return await _reporteRepository.GetAll();
        }

        // 2. Método GetById (Restaurado)
        public async Task<ReporteResponseDTO?> GetById(int id)
        {
            return await _reporteRepository.GetById(id);
        }

        // 3. Método Create (CON LÓGICA DE CORREO)
        public async Task<int> Create(ReporteCreateDTO dto)
        {
            // A. Guardamos en Base de Datos
            int idReporte = await _reporteRepository.Insert(dto);

            // B. Intentamos enviar el correo
            if (idReporte > 0)
            {
                try
                {
                    // DATOS DE PRUEBA (Cámbialos por reales si quieres)
                    string destinatario = "cliente@ejemplo.com";
                    string asunto = $"Nuevo Reporte Generado: {dto.TipoReporte}";
                    string cuerpo = $"Se ha generado un nuevo reporte con formato {dto.Formato}. \n\n Puede verlo en el sistema.";

                    EnviarCorreoReal(destinatario, asunto, cuerpo);
                }
                catch (Exception ex)
                {
                    // Si falla el correo, NO rompemos el programa, solo lo escribimos en consola
                    Console.WriteLine("Error enviando correo: " + ex.Message);
                }
            }

            return idReporte;
        }

        // 4. Método Update (Restaurado)
        public async Task<int> Update(ReporteUpdateDTO dto)
        {
            return await _reporteRepository.Update(dto);
        }

        // 5. Método Delete (Restaurado)
        public async Task<bool> Delete(int id)
        {
            return await _reporteRepository.Delete(id);
        }

        // --- MÉTODO PRIVADO PARA ENVIAR EL CORREO ---
        private void EnviarCorreoReal(string para, string asunto, string cuerpo)
        {
            // CONFIGURACIÓN SMTP (Ejemplo con Gmail)
            // NOTA: Para Gmail necesitas una "Contraseña de Aplicación", no tu clave normal.
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tucorreo@gmail.com", "tu_contraseña_de_aplicacion"),
                EnableSsl = true,
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress("tucorreo@gmail.com", "SLA Tracker System"),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = false,
            };

            mensaje.To.Add(para);

            smtpClient.Send(mensaje);
        }
    }
}