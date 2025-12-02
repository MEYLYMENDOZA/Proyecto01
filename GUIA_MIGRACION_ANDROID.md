# ?? GUÍA RÁPIDA - MIGRAR A ENDPOINT SIMPLIFICADO

## ? YA ESTÁ LISTO EN EL BACKEND

El endpoint `/api/reporte/solicitudes-tendencia` ya está implementado y funcionando.

**URL:** `http://192.168.100.4:5120/api/reporte/solicitudes-tendencia`

---

## ?? CAMBIOS EN ANDROID

### **PASO 1: Actualizar Interface Retrofit**

**Archivo:** `data/remote/ReporteApi.kt` (o como lo tengas nombrado)

```kotlin
interface ReporteApi {
    
    // ? ELIMINAR O COMENTAR el endpoint antiguo
    // @GET("reporte/tendencia")
    // suspend fun getTendencia(...): Response<...>
    
    // ? USAR este nuevo endpoint
    @GET("reporte/solicitudes-tendencia")
    suspend fun getSolicitudesTendencia(
        @Query("anio") anio: Int? = null,
        @Query("tipoSla") tipoSla: String,
        @Query("idArea") idArea: Int? = null
    ): Response<RespuestaSolicitudesTendencia>
    
    // Mantén los otros endpoints
    @GET("reporte/tipos-sla-disponibles")
    suspend fun getTiposSlaDisponibles(): Response<List<TipoSla>>
    
    @GET("reporte/anios-disponibles")
    suspend fun getAniosDisponibles(): Response<List<Int>>
    
    @GET("reporte/meses-disponibles")
    suspend fun getMesesDisponibles(@Query("anio") anio: Int): Response<List<Int>>
}
```

---

### **PASO 2: Crear Modelos de Datos**

**Archivo:** `data/model/TendenciaModels.kt`

```kotlin
package com.example.proyecto1.data.model

import com.google.gson.annotations.SerializedName

/**
 * Respuesta del endpoint /solicitudes-tendencia
 */
data class RespuestaSolicitudesTendencia(
    @SerializedName("tipoSla")
    val tipoSla: String,
    
    @SerializedName("diasUmbral")
    val diasUmbral: Int,
    
    @SerializedName("fechaInicio")
    val fechaInicio: String,
    
    @SerializedName("fechaFin")
    val fechaFin: String,
    
    @SerializedName("totalSolicitudes")
    val totalSolicitudes: Int,
    
    @SerializedName("totalMeses")
    val totalMeses: Int,
    
    @SerializedName("datosMensuales")
    val datosMensuales: List<DatoMensual>
)

/**
 * Datos de un mes específico
 */
data class DatoMensual(
    @SerializedName("año")
    val anio: Int,
    
    @SerializedName("mes")
    val mes: Int,
    
    @SerializedName("mesNombre")
    val mesNombre: String,
    
    @SerializedName("totalCasos")
    val totalCasos: Int,
    
    @SerializedName("cumplidos")
    val cumplidos: Int,
    
    @SerializedName("noCumplidos")
    val noCumplidos: Int,
    
    @SerializedName("porcentajeCumplimiento")
    val porcentajeCumplimiento: Double
)

/**
 * Resultado del análisis de tendencia (calculado en la app)
 */
data class AnalisisTendencia(
    val datos: RespuestaSolicitudesTendencia,
    val regresion: RegresionLineal,
    val proyeccion: Double,
    val tendencia: EstadoTendencia
)

/**
 * Resultado de la regresión lineal
 */
data class RegresionLineal(
    val pendiente: Double,
    val intercepto: Double,
    val r2: Double = 0.0 // Coeficiente de determinación (opcional)
)

/**
 * Estado de la tendencia
 */
enum class EstadoTendencia {
    MEJORANDO,
    EMPEORANDO,
    ESTABLE_ALTO,
    ESTABLE_MEDIO,
    ESTABLE_BAJO,
    SIN_DATOS
}
```

---

### **PASO 3: Actualizar Repository**

**Archivo:** `data/repository/TendenciaRepository.kt`

```kotlin
package com.example.proyecto1.data.repository

import com.example.proyecto1.data.model.*
import com.example.proyecto1.data.remote.ReporteApi
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import kotlin.math.pow
import kotlin.math.sqrt

class TendenciaRepository(private val api: ReporteApi) {
    
    /**
     * Obtener análisis completo de tendencia
     */
    suspend fun obtenerAnalisisTendencia(
        anio: Int?,
        tipoSla: String,
        idArea: Int? = null
    ): Result<AnalisisTendencia> = withContext(Dispatchers.IO) {
        try {
            val response = api.getSolicitudesTendencia(anio, tipoSla, idArea)
            
            if (response.isSuccessful && response.body() != null) {
                val datos = response.body()!!
                
                // Calcular regresión lineal
                val regresion = calcularRegresionLineal(datos.datosMensuales)
                
                // Calcular proyección
                val proyeccion = calcularProyeccion(regresion, datos.totalMeses)
                
                // Determinar tendencia
                val tendencia = determinarTendencia(regresion.pendiente, proyeccion)
                
                Result.success(
                    AnalisisTendencia(
                        datos = datos,
                        regresion = regresion,
                        proyeccion = proyeccion,
                        tendencia = tendencia
                    )
                )
            } else {
                Result.failure(Exception("Error HTTP ${response.code()}: ${response.message()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
    
    /**
     * Calcular regresión lineal simple
     * y = mx + b
     */
    private fun calcularRegresionLineal(datos: List<DatoMensual>): RegresionLineal {
        if (datos.isEmpty()) {
            return RegresionLineal(0.0, 0.0, 0.0)
        }
        
        val n = datos.size.toDouble()
        val x = (1..datos.size).map { it.toDouble() }
        val y = datos.map { it.porcentajeCumplimiento }
        
        val sumX = x.sum()
        val sumY = y.sum()
        val sumXY = x.zip(y).sumOf { it.first * it.second }
        val sumX2 = x.sumOf { it.pow(2) }
        val sumY2 = y.sumOf { it.pow(2) }
        
        // Calcular pendiente (m)
        val pendiente = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX.pow(2))
        
        // Calcular intercepto (b)
        val intercepto = (sumY - pendiente * sumX) / n
        
        // Calcular R² (coeficiente de determinación)
        val yMean = sumY / n
        val ssTotal = y.sumOf { (it - yMean).pow(2) }
        val ssResidual = x.zip(y).sumOf { (xi, yi) ->
            val yPred = pendiente * xi + intercepto
            (yi - yPred).pow(2)
        }
        val r2 = if (ssTotal > 0) 1 - (ssResidual / ssTotal) else 0.0
        
        return RegresionLineal(
            pendiente = pendiente,
            intercepto = intercepto,
            r2 = r2.coerceIn(0.0, 1.0)
        )
    }
    
    /**
     * Calcular proyección para el siguiente período
     */
    private fun calcularProyeccion(regresion: RegresionLineal, totalMeses: Int): Double {
        val xProyeccion = totalMeses + 1.0
        val proyeccion = regresion.pendiente * xProyeccion + regresion.intercepto
        
        // Limitar entre 0 y 100
        return proyeccion.coerceIn(0.0, 100.0)
    }
    
    /**
     * Determinar estado de tendencia
     */
    private fun determinarTendencia(pendiente: Double, proyeccion: Double): EstadoTendencia {
        return when {
            pendiente > 0.5 -> EstadoTendencia.MEJORANDO
            pendiente < -0.5 -> EstadoTendencia.EMPEORANDO
            proyeccion >= 80 -> EstadoTendencia.ESTABLE_ALTO
            proyeccion >= 60 -> EstadoTendencia.ESTABLE_MEDIO
            else -> EstadoTendencia.ESTABLE_BAJO
        }
    }
}
```

---

### **PASO 4: Actualizar ViewModel**

**Archivo:** `ui/tendencia/TendenciaViewModel.kt`

```kotlin
package com.example.proyecto1.ui.tendencia

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.proyecto1.data.model.AnalisisTendencia
import com.example.proyecto1.data.repository.TendenciaRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch

class TendenciaViewModel(
    private val repository: TendenciaRepository
) : ViewModel() {
    
    private val _uiState = MutableStateFlow<TendenciaUiState>(TendenciaUiState.Loading)
    val uiState: StateFlow<TendenciaUiState> = _uiState.asStateFlow()
    
    fun cargarTendencia(anio: Int?, tipoSla: String, idArea: Int? = null) {
        viewModelScope.launch {
            _uiState.value = TendenciaUiState.Loading
            
            repository.obtenerAnalisisTendencia(anio, tipoSla, idArea)
                .onSuccess { analisis ->
                    _uiState.value = TendenciaUiState.Success(analisis)
                }
                .onFailure { error ->
                    _uiState.value = TendenciaUiState.Error(error.message ?: "Error desconocido")
                }
        }
    }
}

sealed class TendenciaUiState {
    object Loading : TendenciaUiState()
    data class Success(val analisis: AnalisisTendencia) : TendenciaUiState()
    data class Error(val message: String) : TendenciaUiState()
}
```

---

### **PASO 5: Actualizar UI (Compose)**

**Archivo:** `ui/tendencia/TendenciaScreen.kt`

```kotlin
@Composable
fun TendenciaScreen(
    viewModel: TendenciaViewModel = viewModel()
) {
    val uiState by viewModel.uiState.collectAsState()
    
    Column(modifier = Modifier.fillMaxSize().padding(16.dp)) {
        when (uiState) {
            is TendenciaUiState.Loading -> {
                CircularProgressIndicator(modifier = Modifier.align(Alignment.CenterHorizontally))
            }
            
            is TendenciaUiState.Success -> {
                val analisis = (uiState as TendenciaUiState.Success).analisis
                
                // Mostrar información
                Text(
                    text = "Análisis de Tendencia - ${analisis.datos.tipoSla}",
                    style = MaterialTheme.typography.headlineMedium
                )
                
                Spacer(modifier = Modifier.height(16.dp))
                
                // KPIs
                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.SpaceEvenly
                ) {
                    KpiCard("Proyección", "${analisis.proyeccion.format(1)}%")
                    KpiCard("Tendencia", analisis.tendencia.name)
                    KpiCard("Total Meses", "${analisis.datos.totalMeses}")
                }
                
                Spacer(modifier = Modifier.height(24.dp))
                
                // Gráfico (usando una librería como MPAndroidChart o Vico)
                GraficoTendencia(analisis.datos.datosMensuales, analisis.regresion)
                
                Spacer(modifier = Modifier.height(16.dp))
                
                // Estadísticas de regresión
                Card(modifier = Modifier.fillMaxWidth()) {
                    Column(modifier = Modifier.padding(16.dp)) {
                        Text("Estadísticas de Regresión")
                        Text("Pendiente: ${analisis.regresion.pendiente.format(4)}")
                        Text("Intercepto: ${analisis.regresion.intercepto.format(2)}")
                        Text("R²: ${analisis.regresion.r2.format(4)}")
                    }
                }
            }
            
            is TendenciaUiState.Error -> {
                Text(
                    text = "Error: ${(uiState as TendenciaUiState.Error).message}",
                    color = Color.Red
                )
            }
        }
    }
}

@Composable
fun KpiCard(label: String, value: String) {
    Card {
        Column(
            modifier = Modifier.padding(16.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(label, style = MaterialTheme.typography.labelMedium)
            Text(value, style = MaterialTheme.typography.headlineSmall)
        }
    }
}

fun Double.format(decimals: Int): String = "%.${decimals}f".format(this)
```

---

## ?? PROBAR

### **1. Probar desde el Navegador**

```
http://192.168.100.4:5120/api/reporte/solicitudes-tendencia?tipoSla=SLA001&anio=2024
```

**Respuesta esperada:**
```json
{
  "tipoSla": "SLA001",
  "diasUmbral": 35,
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
    }
    // ... más meses
  ]
}
```

### **2. Probar desde Android**

1. Actualiza los archivos mencionados
2. Ejecuta la app
3. Selecciona año y tipo SLA
4. La app calculará automáticamente:
   - Regresión lineal
   - Proyección
   - Estado de tendencia
   - Mostrará el gráfico

---

## ? VENTAJAS

? **Backend simple** - Solo consulta y agrupa  
? **App flexible** - Puedes cambiar algoritmos sin tocar backend  
? **Sin tabla adicional** - No necesitas `prediccion_tendencia_log`  
? **Más rápido** - Menos procesamiento en backend  
? **Offline capable** - Puede recalcular sin conexión  

---

## ?? CHECKLIST

- [ ] Actualizar `ReporteApi.kt` con nuevo endpoint
- [ ] Crear modelos de datos (`TendenciaModels.kt`)
- [ ] Actualizar `TendenciaRepository.kt` con cálculos
- [ ] Actualizar `TendenciaViewModel.kt`
- [ ] Actualizar UI (`TendenciaScreen.kt`)
- [ ] Probar endpoint desde navegador
- [ ] Probar desde app Android

---

## ?? ¿NECESITAS AYUDA?

Si tienes dudas sobre:
- Implementación de gráficos (MPAndroidChart o Vico)
- Inyección de dependencias (Hilt/Koin)
- Manejo de estados con Flow
- Testing

¡Pregúntame! ??
