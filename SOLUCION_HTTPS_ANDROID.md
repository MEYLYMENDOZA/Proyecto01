# ?? SOLUCIÓN: Problema HTTPS con Android

## ? EL PROBLEMA

Estás viendo este error porque el emulador de Android **NO puede conectarse** a:
```
https://0.0.0.0:7263
```

### ¿Por qué?

Android **NO CONFÍA** en certificados SSL autofirmados (self-signed certificates) que .NET genera automáticamente para desarrollo local. Esto es una medida de seguridad de Android.

---

## ? LA SOLUCIÓN

### **Para Desarrollo con Android: USA HTTP (NO HTTPS)**

1. **En Visual Studio:**
   - Busca el dropdown que dice "https" o "http" en la barra de herramientas
   - **Selecciona "http"** (NO "https")
   - Presiona F5 para iniciar

2. **Verifica que veas en la consola:**
   ```
   Now listening on: http://localhost:5120
   Now listening on: http://0.0.0.0:5120
   ```

   **NO deberías ver:**
   ```
   Now listening on: https://localhost:7263  ? ? MAL para Android
   ```

3. **En tu aplicación Android, usa:**
   ```kotlin
   // ? CORRECTO - HTTP
   private const val BASE_URL = "http://10.0.2.2:5120/"
   
   // ? INCORRECTO - HTTPS (causará errores)
   // private const val BASE_URL = "https://10.0.2.2:7263/"
   ```

---

## ?? CONFIGURACIÓN APLICADA

### **1. launchSettings.json**

```json
{
  "profiles": {
    "http": {                                    ? Usa este perfil
      "commandName": "Project",
      "applicationUrl": "http://localhost:5120;http://0.0.0.0:5120",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {                                   ? NO uses este perfil con Android
      "commandName": "Project",
      "applicationUrl": "https://localhost:7263;http://localhost:5120",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### **2. Program.cs**

```csharp
// ? CORRECTO - Redirección HTTPS comentada para desarrollo
// app.UseHttpsRedirection();

// ? INCORRECTO - Esto fuerza HTTPS
// app.UseHttpsRedirection();
```

---

## ?? VERIFICACIÓN RÁPIDA

### **Checklist antes de ejecutar tu app Android:**

- [ ] ? Seleccionaste el perfil **"http"** en Visual Studio
- [ ] ? La consola muestra `http://0.0.0.0:5120` (NO https)
- [ ] ? `http://localhost:5120/api/area` funciona en el navegador
- [ ] ? Tu app Android usa `http://10.0.2.2:5120/` (NO https)
- [ ] ? El firewall está configurado para el puerto 5120

---

## ?? ERRORES COMUNES

### **Error 1: "SSL Handshake Failed"**
```
javax.net.ssl.SSLHandshakeException: 
java.security.cert.CertPathValidatorException: Trust anchor for certification path not found
```

**Causa:** Estás intentando usar HTTPS con un certificado autofirmado.

**Solución:** Cambia a HTTP usando el perfil "http".

---

### **Error 2: "Connection timed out to https://..."**
```
failed to connect to /10.0.2.2 (port 7263) after 60000ms
```

**Causa:** Estás usando el puerto HTTPS (7263) en lugar del HTTP (5120).

**Solución:** 
1. Usa el perfil "http" en Visual Studio
2. Cambia tu app Android a usar `http://10.0.2.2:5120/`

---

### **Error 3: API redirige a HTTPS automáticamente**

**Causa:** `app.UseHttpsRedirection()` está habilitado.

**Solución:** Verifica que esté comentado en `Program.cs`:
```csharp
// app.UseHttpsRedirection();  ? Debe estar comentado
```

---

## ?? CONFIGURACIÓN EN PRODUCCIÓN

Cuando despliegues tu API en producción:

### **Opción 1: Usar HTTPS con certificado válido**
```csharp
// En producción, habilita la redirección HTTPS
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
```

### **Opción 2: Configurar Android para confiar en tu certificado**

Para desarrollo, puedes configurar Android para que confíe en certificados autofirmados:

**AndroidManifest.xml:**
```xml
<application
    android:networkSecurityConfig="@xml/network_security_config"
    ...>
```

**res/xml/network_security_config.xml:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <base-config cleartextTrafficPermitted="true">
        <trust-anchors>
            <certificates src="system" />
            <certificates src="user" />
        </trust-anchors>
    </base-config>
    
    <!-- Solo para desarrollo - ELIMINAR en producción -->
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="true">10.0.2.2</domain>
        <domain includeSubdomains="true">localhost</domain>
    </domain-config>
</network-security-config>
```

**?? ADVERTENCIA:** Esto es SOLO para desarrollo. En producción, usa HTTPS con un certificado válido.

---

## ?? EXPLICACIÓN TÉCNICA

### ¿Por qué Android bloquea certificados autofirmados?

1. **Seguridad:** Los certificados autofirmados no son validados por una Autoridad Certificadora (CA) confiable
2. **Ataques Man-in-the-Middle:** Un atacante podría interceptar el tráfico
3. **Política de Android:** Desde Android 9+, el tráfico de texto plano (HTTP) está restringido por defecto

### ¿Por qué usar HTTP en desarrollo?

1. **Simplicidad:** No necesitas configurar certificados
2. **Velocidad:** Desarrollo más rápido sin problemas de SSL
3. **Debugging:** Más fácil inspeccionar el tráfico HTTP

### ¿Cuándo usar HTTPS?

1. **Producción:** SIEMPRE usa HTTPS en producción
2. **Datos sensibles:** Si manejas contraseñas, tokens, etc.
3. **APIs públicas:** Cualquier API accesible desde internet

---

## ?? COMANDOS DE DIAGNÓSTICO

### **Verificar qué puertos están escuchando:**
```powershell
netstat -ano | Select-String ":5120"
netstat -ano | Select-String ":7263"
```

**Deberías ver:**
```
TCP    0.0.0.0:5120           0.0.0.0:0              LISTENING       12345
TCP    127.0.0.1:5120         0.0.0.0:0              LISTENING       12345
```

**NO deberías ver (cuando usas perfil http):**
```
TCP    0.0.0.0:7263           0.0.0.0:0              LISTENING       12345  ? ?
```

### **Probar endpoint HTTP:**
```powershell
Invoke-WebRequest -Uri "http://localhost:5120/api/area" -Method GET
```

**Debe funcionar sin errores de SSL.**

---

## ?? RESUMEN RÁPIDO

| Aspecto | Desarrollo con Android | Producción |
|---------|----------------------|------------|
| Perfil de Visual Studio | `http` | `https` |
| URL de la API | `http://0.0.0.0:5120` | `https://tu-dominio.com` |
| UseHttpsRedirection | `// comentado` | `habilitado` |
| URL en Android (emulador) | `http://10.0.2.2:5120/` | `https://tu-dominio.com/` |
| URL en Android (físico) | `http://TU_IP:5120/` | `https://tu-dominio.com/` |
| Certificado SSL | No necesario | Requerido (válido) |

---

## ? PASOS FINALES

1. **Detén tu API** si está corriendo
2. **Selecciona el perfil "http"** en Visual Studio
3. **Presiona F5** para iniciar
4. **Verifica** que solo veas `http://` en la consola (NO `https://`)
5. **Configura el firewall** (una sola vez):
   ```powershell
   netsh advfirewall firewall add rule name="ASP.NET Core API - Port 5120" dir=in action=allow protocol=TCP localport=5120
   ```
6. **Prueba en el navegador:** `http://localhost:5120/api/area`
7. **Prueba en Android:** `http://10.0.2.2:5120/api/area`

---

**Última actualización:** 2025-11-25  
**Puerto HTTP:** 5120  
**Puerto HTTPS (NO usar con Android):** 7263
