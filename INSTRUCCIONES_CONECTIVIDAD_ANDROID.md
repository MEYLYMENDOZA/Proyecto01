# ?? SOLUCIÓN: Conectividad Android con API .NET

## ?? ESTADO ACTUAL (VERIFICADO)

**Tu aplicación Android NO se puede conectar porque:**

? Configuración de código: **CORRECTA**  
? CORS configurado: **CORRECTA**  
? launchSettings.json: **CORRECTA**  
? UseHttpsRedirection: **DESHABILITADO PARA DESARROLLO** ?  
? **LA API DEBE ESTAR EJECUTÁNDOSE CON EL PERFIL "http"**  
? Firewall: **NECESITA CONFIGURACIÓN**

**?? IMPORTANTE:** 
- Android NO puede conectarse a `https://0.0.0.0:7263` porque no confía en certificados autofirmados
- Debes usar el perfil **"http"** (NO "https") cuando desarrolles con Android
- El perfil "http" usa solo `http://localhost:5120` y `http://0.0.0.0:5120`

---

## ?? PROBLEMA IDENTIFICADO

Tu aplicación Android no puede conectarse a la API porque:
1. ? **LA API NO ESTÁ CORRIENDO** (puerto 5120 cerrado) ? **PROBLEMA PRINCIPAL**
2. ? **No hay regla de firewall** para permitir conexiones entrantes
3. ?? La configuración de `launchSettings.json` está correcta, pero **LA API DEBE ESTAR EJECUTÁNDOSE**

**IMPORTANTE:** No importa cuántas veces reinicies tu app Android, **nunca se conectará** hasta que inicies la API en tu PC.

---

## ? SOLUCIÓN ULTRA RÁPIDA (HACER AHORA)

### **Opción 1: Desde Visual Studio (MÁS FÁCIL)**

1. **Asegúrate de que Visual Studio esté abierto** con tu solución `Proyecto01.sln`
2. **En la barra de herramientas superior**, localiza el dropdown que dice "https" o "http"
3. **Selecciona "http"** (NO "https")
4. **Presiona F5** o haz clic en el botón verde ??
5. **Espera** a que aparezca una ventana de consola negra con el texto:
   ```
   Now listening on: http://localhost:5120
   Now listening on: http://0.0.0.0:5120
   Application started. Press Ctrl+C to shut down.
   ```
6. **NO CIERRES ESA VENTANA** - La API debe permanecer ejecutándose

### **Opción 2: Desde PowerShell (Script automático)**

Abre PowerShell en la carpeta del proyecto y ejecuta:
```powershell
.\INICIAR_API.ps1
```

### **Opción 3: Desde Terminal manualmente**

```powershell
cd "D:\REPOS\Proyecto movil backend\Proyecto01.API"
dotnet run --launch-profile http
```

**?? IMPORTANTE:** Deja la ventana abierta mientras usas tu app Android. Si cierras la ventana, la API se detendrá.

---

## ?? CONFIGURAR FIREWALL (UNA SOLA VEZ)

Abre PowerShell **COMO ADMINISTRADOR** (clic derecho ? "Ejecutar como administrador"):

```powershell
netsh advfirewall firewall add rule name="ASP.NET Core API - Port 5120" dir=in action=allow protocol=TCP localport=5120
```

Deberías ver: `Correcto.` o `Ok.`

---

## ? VERIFICAR QUE TODO FUNCIONE

### **Paso 1: Verificar que la API esté corriendo**

Ejecuta este script:
```powershell
.\VERIFICAR_CONECTIVIDAD.ps1
```

O manualmente:
```powershell
netstat -ano | Select-String ":5120"
```

**Deberías ver algo como:**
```
TCP    0.0.0.0:5120           0.0.0.0:0              LISTENING       12345
TCP    127.0.0.1:5120         0.0.0.0:0              LISTENING       12345
```

### **Paso 2: Probar desde el navegador**

Abre tu navegador y visita:
```
http://localhost:5120/api/area
```

**Deberías ver:** Un JSON con datos de áreas (o un array vacío `[]`)

### **Paso 3: Probar desde Android**

**AHORA SÍ** tu app Android debería conectarse usando:
```
http://10.0.2.2:5120/api/sla/solicitudes?meses=12
```

---

## ? SOLUCIÓN PASO A PASO (DETALLADA)

### **PASO 1: Iniciar la API**

Desde Visual Studio:

1. **Abre tu solución** en Visual Studio 2022
2. En el menú superior, busca el dropdown de perfiles de ejecución
3. Selecciona el perfil **"http"** (NO "https")
4. Presiona **F5** o haz clic en el botón ?? **Play** para iniciar

**Verificar que esté corriendo:**
- En la consola de salida deberías ver:
  ```
  Now listening on: http://localhost:5120
  Now listening on: http://0.0.0.0:5120
  Application started. Press Ctrl+C to shut down.
  ```

**Alternativa desde Terminal:**
```powershell
cd "D:\REPOS\Proyecto movil backend\Proyecto01.API"
dotnet run --launch-profile http
```

---

### **PASO 2: Configurar el Firewall de Windows**

**IMPORTANTE:** Ejecuta PowerShell como **Administrador**

```powershell
# Agregar regla para permitir conexiones entrantes al puerto 5120
netsh advfirewall firewall add rule name="ASP.NET Core API - Port 5120" dir=in action=allow protocol=TCP localport=5120

# Verificar que la regla se creó
netsh advfirewall firewall show rule name="ASP.NET Core API - Port 5120"
```

**Si necesitas eliminar la regla más tarde:**
```powershell
netsh advfirewall firewall delete rule name="ASP.NET Core API - Port 5120"
```

---

### **PASO 3: Verificar la Conectividad**

#### **Desde tu PC (navegador o Postman):**
```
http://localhost:5120/api/area
```

#### **Obtener tu IP local:**
```powershell
ipconfig | Select-String -Pattern "IPv4"
```
Tu IP es: **10.56.242.155**

#### **Desde el navegador del emulador Android:**
```
http://10.0.2.2:5120/api/area
```

#### **Desde un dispositivo físico Android (misma WiFi):**
```
http://10.56.242.155:5120/api/area
```

---

### **PASO 4: Configurar tu Aplicación Android**

En tu código Android, actualiza la URL base:

```kotlin
// Para EMULADOR Android
private const val BASE_URL = "http://10.0.2.2:5120/"

// Para DISPOSITIVO FÍSICO en la misma red WiFi
// private const val BASE_URL = "http://10.56.242.155:5120/"
```

**Ejemplo completo de Retrofit:**
```kotlin
object RetrofitInstance {
    private const val BASE_URL = "http://10.0.2.2:5120/"
    
    private val loggingInterceptor = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }
    
    private val client = OkHttpClient.Builder()
        .addInterceptor(loggingInterceptor)
        .connectTimeout(60, TimeUnit.SECONDS)
        .readTimeout(60, TimeUnit.SECONDS)
        .writeTimeout(60, TimeUnit.SECONDS)
        .build()
    
    val api: ApiService by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(client)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
            .create(ApiService::class.java)
    }
}
```

---

## ?? DIAGNÓSTICO DE PROBLEMAS

### **Verificar que la API esté corriendo:**
```powershell
netstat -ano | Select-String ":5120"
```
**Deberías ver algo como:**
```
TCP    0.0.0.0:5120           0.0.0.0:0              LISTENING       12345
TCP    127.0.0.1:5120         0.0.0.0:0              LISTENING       12345
```

### **Probar conectividad desde PowerShell:**
```powershell
# Debe devolver "True" si la API está corriendo
Test-NetConnection -ComputerName localhost -Port 5120 -InformationLevel Quiet
```

### **Hacer una petición de prueba:**
```powershell
Invoke-WebRequest -Uri "http://localhost:5120/api/area" -Method GET
```

---

## ?? CHECKLIST DE VERIFICACIÓN

- [ ] ? La API está corriendo (Visual Studio o `dotnet run`)
- [ ] ? El puerto 5120 está abierto (`netstat -ano | Select-String ":5120"`)
- [ ] ? La regla de firewall está creada
- [ ] ? CORS está configurado en `Program.cs` (ya está configurado ?)
- [ ] ? `launchSettings.json` tiene `http://0.0.0.0:5120` (ya está configurado ?)
- [ ] ? Puedes acceder desde el navegador: `http://localhost:5120/api/area`
- [ ] ? La app Android usa `http://10.0.2.2:5120` para emulador
- [ ] ?? PC y dispositivo Android están en la misma red WiFi (solo para dispositivo físico)

---

## ? SOLUCIÓN RÁPIDA

**Si solo quieres probar rápidamente:**

1. **Inicia la API desde Visual Studio:**
   - Selecciona perfil "http"
   - Presiona F5

2. **Configura el firewall (PowerShell como Admin):**
   ```powershell
   netsh advfirewall firewall add rule name="ASP.NET Core API - Port 5120" dir=in action=allow protocol=TCP localport=5120
   ```

3. **En tu app Android, usa:**
   ```kotlin
   http://10.0.2.2:5120/api/sla/solicitudes?meses=12
   ```

4. **Verifica en el navegador:**
   ```
   http://localhost:5120/api/area
   ```

---

## ?? ERRORES COMUNES

### **Error: "Connection timed out"**
- ? La API **NO** está corriendo ? Iniciar con F5 en Visual Studio
- ? Firewall bloqueando ? Agregar regla del firewall
- ? URL incorrecta ? Usar `10.0.2.2` para emulador, no `localhost`

### **Error: "Connection refused"**
- ? Puerto incorrecto ? Verificar que sea 5120
- ? API corriendo en HTTPS en lugar de HTTP ? **Usar perfil "http" NO "https"**
- ? Estás usando el perfil "https" ? Cambia a "http" en Visual Studio

### **Error: "SSL Handshake failed" o "Certificate error"**
- ? Estás usando HTTPS ? **Android no confía en certificados autofirmados**
- ? Cambia a HTTP en desarrollo ? Usa el perfil "http" en Visual Studio
- ? Verifica que tu app Android use `http://` NO `https://`

### **Error: "Unable to resolve host"**
- ? Problema de DNS ? Usar IP directa: `10.56.242.155:5120`
- ? No estás en la misma red WiFi ? Conectar a la misma red

---

## ?? CONFIGURACIÓN PARA PRODUCCIÓN

**Cuando despliegues a producción:**

1. **Cambiar CORS a orígenes específicos:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://tuapp.com", "https://api.tuapp.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

2. **Usar HTTPS:**
```json
"applicationUrl": "https://0.0.0.0:7263"
```

3. **Certificado SSL válido**

---

## ?? SOPORTE

Si después de seguir estos pasos sigues teniendo problemas:

1. Verifica que la API esté corriendo: `netstat -ano | Select-String ":5120"`
2. Revisa los logs de la API en la consola de Visual Studio
3. Revisa los logs de Android en Logcat
4. Prueba desde el navegador de tu PC primero: `http://localhost:5120/api/area`

---

**Última actualización:** 2025-11-25  
**IP Local detectada:** 10.56.242.155  
**Puerto de la API:** 5120
