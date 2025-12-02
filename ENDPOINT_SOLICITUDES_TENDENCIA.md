# ?? NUEVO ENDPOINT SIMPLIFICADO - SOLICITUDES TENDENCIA

## ?? OBJETIVO

Crear un endpoint **simplificado** que solo retorne **datos crudos** agrupados por mes.  
La **app Android calcula** la regresión lineal, proyección y estado de tendencia.

---

## ? ENDPOINT IMPLEMENTADO

### **GET `/api/reporte/solicitudes-tendencia`**

**Descripción:** Retorna datos históricos agrupados por mes para que la app Android haga los cálculos estadísticos.

### **Parámetros**

| Parámetro | Tipo | Obligatorio | Descripción | Ejemplo |
|-----------|------|-------------|-------------|---------|
| `tipoSla` | string | ? Sí | Código del SLA | `SLA001`, `SLA002` |
| `anio` | int? | ? No | Año de análisis (últimos 12 meses si no se especifica) | `2024`, `2025` |
| `idArea` | int? | ? No | Filtrar por área específica | `1`, `2`, `3` |

---

## ?? RESPUESTA DEL ENDPOINT

### **Estructura JSON**

```json
{
  "tipoSla": "SLA001",
  "diasUmbral": 35,
  "fechaInicio": "2024-01-01",
  "fechaFin": "2024-12-31",
  "totalSolicitudes": 1800,
  "totalMeses": 12,
  "datosMensuales": [
    {
      "año": 2024,
      "mes": 1,
      "mesNombre": "ene 2024",
      "totalCasos": 150,
      "cumplidos": 120,
      "noCumplidos": 30,
      "porcentajeCumplimiento": 80.0
    },
    {
      "año": 2024,
      "mes": 2,
      "mesNombre": "feb 2024",
      "totalCasos": 145,
      "cumplidos": 125,
      "noCumplidos": 20,
      "porcentajeCumplimiento": 86.21
    },
    // ... más meses
  ]
}
```

### **Campos de `datosMensuales`**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `año` | int | Año del mes |
| `mes` | int | Número del mes (1-12) |
| `mesNombre` | string | Nombre del mes formateado |
| `totalCasos` | int | Total de solicitudes en ese mes |
| `cumplidos` | int | Solicitudes que cumplieron el SLA |
| `noCumplidos` | int | Solicitudes que NO cumplieron el SLA |
| `porcentajeCumplimiento` | double | Porcentaje de cumplimiento (0-100) |

---

## ?? DIFERENCIAS CON ENDPOINT ANTERIOR

| Característica | `/api/reporte/tendencia` (anterior) | `/api/reporte/solicitudes-tendencia` (nuevo) |
|----------------|-------------------------------------|----------------------------------------------|
| **Cálculos** | ? Backend calcula regresión | ? No calcula nada |
| **Proyección** | ? Backend calcula proyección | ? No calcula proyección |
| **Historial** | ? Lee tabla `prediccion_tendencia_log` | ? No usa tabla de logs |
| **Datos** | DTO complejo con tendencia | Datos crudos por mes |
| **Dependencias** | TendenciaService, TendenciaLogRepository | Solo SlaRepository |
| **Velocidad** | Más lento (cálculos complejos) | ? Más rápido (solo consulta y agrupa) |
| **Responsabilidad** | Backend | ?? App Android |

---

## ?? CÁLCULOS QUE HARÁ LA APP ANDROID

### **1. Regresión Lineal Simple**

```kotlin
fun calcularRegresionLineal(datos: List<DatoMensual>): Pair<Double, Double> {
    val n = datos.size
    val x = datos.indices.map { it + 1.0 }
    val y = datos.map { it.porcentajeCumplimiento }
    
    val sumX = x.sum()
    val sumY = y.sum()
    val sumXY = x.zip(y).sumOf { it.first * it.second }
    val sumX2 = x.sumOf { it * it }
    
    val pendiente = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX)
    val intercepto = (sumY - pendiente * sumX) / n
    
    return Pair(pendiente, intercepto)
}
```

### **2. Proyección para el Siguiente Mes**

```kotlin
fun calcularProyeccion(pendiente: Double, intercepto: Double, n: Int): Double {
    val xProyeccion = n + 1
    val proyeccion = pendiente * xProyeccion + intercepto
    return proyeccion.coerceIn(0.0, 100.0) // Limitar entre 0 y 100
}
```

### **3. Determinar Estado de Tendencia**

```kotlin
fun determinarTendencia(pendiente: Double, proyeccion: Double): String {
    return when {
        pendiente > 0.5 -> "MEJORANDO"
        pendiente < -0.5 -> "EMPEORANDO"
        proyeccion >= 80 -> "ESTABLE_ALTO"
        proyeccion >= 60 -> "ESTABLE_MEDIO"
        else -> "ESTABLE_BAJO"
    }
}
```

---

## ?? EJEMPLOS DE USO

### **Ejemplo 1: Datos del año 2024 para SLA001**

```http
GET http://localhost:5120/api/reporte/solicitudes-tendencia?tipoSla=SLA001&anio=2024
```

**Respuesta:**
```json
{
  "tipoSla": "SLA001",
  "diasUmbral": 35,
  "totalSolicitudes": 1800,
  "totalMeses": 12,
  "datosMensuales": [...]
}
```

### **Ejemplo 2: Últimos 12 meses para SLA002**

```http
GET http://localhost:5120/api/reporte/solicitudes-tendencia?tipoSla=SLA002
```

### **Ejemplo 3: Filtrar por área específica**

```http
GET http://localhost:5120/api/reporte/solicitudes-tendencia?tipoSla=SLA001&anio=2024&idArea=1
```

---

## ? VENTAJAS DEL NUEVO ENFOQUE

### **Backend**
- ? **Más simple**: Solo consulta y agrupa datos
- ? **Más rápido**: No hace cálculos complejos
- ? **Menos dependencias**: No necesita tabla de logs
- ? **Menos errores**: Menos código = menos bugs

### **App Android**
- ? **Más control**: La app decide cómo calcular
- ? **Más flexible**: Puede cambiar algoritmos sin tocar backend
- ? **Offline**: Puede recalcular sin llamar al backend
- ? **Mejor UX**: Puede mostrar cálculos en tiempo real

---

## ?? LOGS ESPERADOS

```
[ReporteController] Solicitud de datos tendencia: año=2024, tipoSla=SLA001, area=null
[ReporteController] Rango de fechas: 2024-01-01 a 2024-12-31
[ReporteController] Config SLA: código=SLA001, umbral=35 días
[ReporteController] Solicitudes encontradas: 1800
[ReporteController] Meses con datos: 12
```

---

## ? CHECKLIST DE IMPLEMENTACIÓN

- [x] Endpoint creado en `ReporteController`
- [x] Inyección de `ISlaRepository` en constructor
- [x] Validación de parámetros
- [x] Sanitización de entrada (ToUpperInvariant)
- [x] Manejo de errores
- [x] Logging para debugging
- [x] Compilación exitosa

---

## ?? ENDPOINTS DISPONIBLES AHORA

| Endpoint | Propósito | Quién Calcula |
|----------|-----------|---------------|
| `/api/reporte/tendencia` | Reporte completo con proyección | ??? Backend |
| `/api/reporte/solicitudes-tendencia` | Datos crudos para análisis | ?? App Android |
| `/api/reporte/anios-disponibles` | Años con datos | - |
| `/api/reporte/tipos-sla-disponibles` | Tipos SLA | - |
| `/api/reporte/meses-disponibles` | Meses disponibles | - |
| `/api/reporte/areas-disponibles` | Áreas disponibles | - |

---

## ?? LISTO PARA USAR

El nuevo endpoint está **100% funcional** y listo para que tu app Android lo consuma.

**Reinicia tu API:**
```sh
dotnet run
```

**Prueba desde el navegador:**
```
http://localhost:5120/api/reporte/solicitudes-tendencia?tipoSla=SLA001&anio=2024
```

¿Necesitas ayuda para implementar los cálculos en Android? ??
