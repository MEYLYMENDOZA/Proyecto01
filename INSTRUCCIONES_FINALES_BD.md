# ?? GUÍA COMPLETA - CONFIGURACIÓN FINAL BASE DE DATOS

## ? TODO LISTO EN EL CÓDIGO

El código de tu API está **100% funcional**. Solo falta crear la tabla en la base de datos.

---

## ?? PASOS PARA COMPLETAR LA CONFIGURACIÓN

### **PASO 1: Crear la Tabla en SQL Server**

1. **Abre SQL Server Management Studio (SSMS)**
2. **Conéctate a tu base de datos** (la que uses para Proyecto01)
3. **Abre el archivo:** `CREATE_TABLE_prediccion_tendencia_log.sql`
4. **Ejecuta el script completo** (F5 o botón Ejecutar)

**? Resultado esperado:**
```
====================================
? TABLA CREADA EXITOSAMENTE
====================================
Tabla: prediccion_tendencia_log
Índices: 4 índices creados para optimizar consultas
```

---

### **PASO 2: Insertar Datos de Prueba (Opcional pero Recomendado)**

1. **Abre el archivo:** `INSERT_DATOS_PRUEBA_tendencia.sql`
2. **Ejecuta el script completo**

**? Resultado esperado:**
```
====================================
? DATOS INSERTADOS EXITOSAMENTE
====================================

Tipo SLA | Total Registros | Año Inicio | Año Fin | Promedio % | Min % | Max %
---------|-----------------|------------|---------|------------|-------|-------
SLA001   | 13              | 2024       | 2025    | 85.5       | 70.00 | 93.75
SLA002   | 8               | 2024       | 2025    | 72.8       | 50.00 | 90.00
```

---

### **PASO 3: Verificar la Tabla**

Ejecuta este query para confirmar que la tabla existe:

```sql
-- Verificar estructura
SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS Tipo,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'prediccion_tendencia_log'
ORDER BY ORDINAL_POSITION;

-- Verificar datos (si insertaste datos de prueba)
SELECT TOP 5 * 
FROM prediccion_tendencia_log 
ORDER BY creado_en DESC;
```

---

## ?? PASO 4: Probar la API

### **4.1 Reiniciar la API**

```sh
# Detén la API si está corriendo (Ctrl+C)
dotnet run
```

### **4.2 Probar desde el Navegador**

Abre en tu navegador:

```
http://localhost:5120/api/reporte/tipos-sla-disponibles
http://localhost:5120/api/reporte/anios-disponibles
http://localhost:5120/api/reporte/tendencia?tipoSla=SLA001&anio=2024
```

### **4.3 Probar desde Android**

1. Abre tu app Android
2. Selecciona:
   - **Año:** 2024
   - **Tipo SLA:** SLA001
3. Presiona **"Generar Reporte"**

---

## ?? RESPUESTA ESPERADA

Si todo está correcto, deberías ver:

```json
{
  "historico": [
    {
      "mes": 1,
      "anio": 2024,
      "porcentajeCumplimiento": 70.00
    },
    {
      "mes": 2,
      "anio": 2024,
      "porcentajeCumplimiento": 71.43
    },
    // ... más meses
  ],
  "tendencia": [70.00, 71.43, 74.55, 78.26, 80.00, ...],
  "proyeccion": 94.80,
  "pendiente": 2.15,
  "intercepto": 68.50,
  "estadoTendencia": "MEJORANDO",
  "metadata": {
    "totalRegistros": 155,
    "fechaGeneracion": "2025-11-27T..."
  }
}
```

---

## ?? LOGS ESPERADOS EN EL SERVIDOR

```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (Xms)
      SELECT [s].* FROM [solicitud] AS [s]
      INNER JOIN [config_sla] AS [c] ON [s].[id_sla] = [c].[id_sla]
      WHERE UPPER([c].[codigo_sla]) = 'SLA001' ...

[TendenciaService] Tipo SLA recibido: SLA001
[TendenciaService] Buscando solicitudes - Tipo SLA: SLA001, Mes: 11, Año: 2024
[TendenciaService] Solicitudes encontradas: 150
[TendenciaService] Días umbral para SLA001: 35

info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (Xms)
      SELECT TOP(12) * FROM [prediccion_tendencia_log]
      WHERE [tipo_sla] = 'SLA001'
      ORDER BY [fecha_analisis] DESC

? Reporte generado exitosamente
```

---

## ?? ENDPOINTS DISPONIBLES

| Endpoint | Método | Descripción | Ejemplo |
|----------|--------|-------------|---------|
| `/api/reporte/anios-disponibles` | GET | Años con solicitudes | `[2025, 2024, 2023]` |
| `/api/reporte/meses-disponibles` | GET | Meses de un año | `?anio=2024` |
| `/api/reporte/areas-disponibles` | GET | Áreas con solicitudes | Lista de áreas |
| `/api/reporte/tipos-sla-disponibles` | GET | Tipos SLA configurados | SLA001, SLA002 |
| `/api/reporte/tendencia` | GET | Reporte de tendencia | Datos + proyección |

---

## ? CHECKLIST FINAL

- [ ] Ejecutar `CREATE_TABLE_prediccion_tendencia_log.sql` en SQL Server
- [ ] Ejecutar `INSERT_DATOS_PRUEBA_tendencia.sql` (opcional)
- [ ] Verificar que la tabla existe con el query de verificación
- [ ] Reiniciar la API con `dotnet run`
- [ ] Probar endpoint desde navegador
- [ ] Probar desde app Android
- [ ] Verificar logs del servidor (sin errores)

---

## ?? SI TIENES PROBLEMAS

### **Error: "Invalid object name 'prediccion_tendencia_log'"**
**Solución:** La tabla no existe. Ejecuta el script `CREATE_TABLE_prediccion_tendencia_log.sql`

### **Error: "There is already an object named..."**
**Solución:** La tabla ya existe. Puedes:
- Omitir este error (la tabla ya está creada)
- O descomentar las líneas de DROP en el script

### **Respuesta: "estadoTendencia": "SIN_DATOS"**
**Solución:** No hay datos históricos. Ejecuta `INSERT_DATOS_PRUEBA_tendencia.sql`

---

## ?? ¡LISTO!

Una vez completados estos pasos, tu API de tendencias estará **100% funcional**.

**Archivos creados:**
- ? `CREATE_TABLE_prediccion_tendencia_log.sql` - Crear tabla
- ? `INSERT_DATOS_PRUEBA_tendencia.sql` - Datos de prueba
- ? `DIAGNOSTICO_TENDENCIA.sql` - Diagnóstico (ya existía)

¿Necesitas ayuda con algún paso específico?
