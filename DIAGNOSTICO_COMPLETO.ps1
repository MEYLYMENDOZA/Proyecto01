# Script de Diagnóstico Avanzado para Android
# Este script verifica TODOS los aspectos necesarios para la conectividad

Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host "  DIAGNÓSTICO COMPLETO - CONECTIVIDAD ANDROID CON API .NET" -ForegroundColor Cyan
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""

$hasErrors = $false

# 1. Verificar si la API está corriendo
Write-Host "1??  VERIFICANDO SI LA API ESTÁ CORRIENDO..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$apiRunning = netstat -ano | Select-String ":5120.*LISTENING"

if ($apiRunning) {
    Write-Host "? La API ESTÁ CORRIENDO en el puerto 5120" -ForegroundColor Green
    Write-Host ""
    $apiRunning | ForEach-Object {
        Write-Host "   $($_.ToString().Trim())" -ForegroundColor Gray
    }
    Write-Host ""
} else {
    Write-Host "? LA API NO ESTÁ CORRIENDO" -ForegroundColor Red
    Write-Host "   ? ACCIÓN REQUERIDA: Inicia la API con F5 en Visual Studio (perfil 'http')" -ForegroundColor Yellow
    Write-Host "   ? O ejecuta: .\INICIAR_API.ps1" -ForegroundColor Yellow
    Write-Host ""
    $hasErrors = $true
}

# 2. Verificar conectividad local
Write-Host "2??  PROBANDO CONECTIVIDAD LOCAL (localhost:5120)..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

if ($apiRunning) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5120/api/area" -Method GET -TimeoutSec 5 -ErrorAction Stop
        Write-Host "? API responde correctamente en localhost" -ForegroundColor Green
        Write-Host "   Status Code: $($response.StatusCode)" -ForegroundColor Gray
        Write-Host "   Content Type: $($response.Headers.'Content-Type')" -ForegroundColor Gray
        Write-Host ""
    } catch {
        Write-Host "? ERROR al conectarse a localhost:5120" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host ""
        $hasErrors = $true
    }
} else {
    Write-Host "??  OMITIDO (API no está corriendo)" -ForegroundColor Yellow
    Write-Host ""
}

# 3. Verificar que escuche en 0.0.0.0
Write-Host "3??  VERIFICANDO QUE LA API ESCUCHE EN TODAS LAS INTERFACES (0.0.0.0)..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$listeningOnAll = netstat -ano | Select-String "0.0.0.0:5120.*LISTENING"

if ($listeningOnAll) {
    Write-Host "? La API ESTÁ escuchando en todas las interfaces (0.0.0.0:5120)" -ForegroundColor Green
    Write-Host "   Esto permite que el emulador Android se conecte usando 10.0.2.2" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "? La API NO escucha en 0.0.0.0" -ForegroundColor Red
    Write-Host "   ? Verifica que launchSettings.json tenga: `"http://0.0.0.0:5120`"" -ForegroundColor Yellow
    Write-Host "   ? Asegúrate de usar el perfil 'http' al iniciar" -ForegroundColor Yellow
    Write-Host ""
    $hasErrors = $true
}

# 4. Verificar regla de firewall
Write-Host "4??  VERIFICANDO REGLA DE FIREWALL WINDOWS..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

try {
    $firewallRule = netsh advfirewall firewall show rule name="ASP.NET Core API - Port 5120" 2>&1
    
    if ($firewallRule -like "*ASP.NET Core API - Port 5120*" -and $firewallRule -like "*Habilitado:*Sí*") {
        Write-Host "? Regla de firewall CONFIGURADA y HABILITADA" -ForegroundColor Green
        Write-Host ""
    } else {
        Write-Host "? Regla de firewall NO encontrada o deshabilitada" -ForegroundColor Red
        Write-Host "   ? ACCIÓN REQUERIDA (PowerShell como ADMINISTRADOR):" -ForegroundColor Yellow
        Write-Host "   netsh advfirewall firewall add rule name=`"ASP.NET Core API - Port 5120`" dir=in action=allow protocol=TCP localport=5120" -ForegroundColor Cyan
        Write-Host ""
        $hasErrors = $true
    }
} catch {
    Write-Host "??  No se pudo verificar la regla de firewall" -ForegroundColor Yellow
    Write-Host ""
}

# 5. Obtener IPs locales
Write-Host "5??  OBTENIENDO DIRECCIONES IP LOCALES..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$ipAddresses = Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
    $_.IPAddress -ne "127.0.0.1" -and 
    $_.PrefixOrigin -ne "WellKnown"
} | Select-Object -ExpandProperty IPAddress

if ($ipAddresses) {
    Write-Host "? IPs locales encontradas:" -ForegroundColor Green
    foreach ($ip in $ipAddresses) {
        Write-Host "   ?? $ip" -ForegroundColor Cyan
        Write-Host "      ? Para dispositivo Android físico: http://$ip`:5120/" -ForegroundColor Gray
    }
    Write-Host ""
    $mainIp = $ipAddresses[0]
} else {
    Write-Host "??  No se encontraron IPs locales válidas" -ForegroundColor Yellow
    Write-Host ""
    $mainIp = "TU_IP_LOCAL"
}

# 6. Probar acceso desde IP local
Write-Host "6??  PROBANDO ACCESO DESDE IP LOCAL ($mainIp)..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

if ($apiRunning -and $mainIp -ne "TU_IP_LOCAL") {
    try {
        $response = Invoke-WebRequest -Uri "http://$mainIp`:5120/api/area" -Method GET -TimeoutSec 5 -ErrorAction Stop
        Write-Host "? API es accesible desde IP local $mainIp" -ForegroundColor Green
        Write-Host "   Status Code: $($response.StatusCode)" -ForegroundColor Gray
        Write-Host ""
    } catch {
        Write-Host "? NO se puede acceder desde IP local" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
        Write-Host "   ? Esto indica un problema de firewall o configuración de red" -ForegroundColor Yellow
        Write-Host ""
        $hasErrors = $true
    }
} else {
    Write-Host "??  OMITIDO (API no está corriendo o IP no disponible)" -ForegroundColor Yellow
    Write-Host ""
}

# 7. Verificar configuración de Kestrel
Write-Host "7??  VERIFICANDO CONFIGURACIÓN DE KESTREL..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$launchSettings = Get-Content "Proyecto01.API\Properties\launchSettings.json" -Raw | ConvertFrom-Json

if ($launchSettings.profiles.http.applicationUrl -like "*0.0.0.0:5120*") {
    Write-Host "? launchSettings.json configurado correctamente" -ForegroundColor Green
    Write-Host "   applicationUrl: $($launchSettings.profiles.http.applicationUrl)" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "? launchSettings.json NO tiene 0.0.0.0:5120" -ForegroundColor Red
    Write-Host "   Configuración actual: $($launchSettings.profiles.http.applicationUrl)" -ForegroundColor Yellow
    Write-Host ""
    $hasErrors = $true
}

# 8. Verificar antivirus/software de seguridad
Write-Host "8??  VERIFICANDO SOFTWARE DE SEGURIDAD..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$antivirusProducts = Get-CimInstance -Namespace root/SecurityCenter2 -ClassName AntivirusProduct -ErrorAction SilentlyContinue

if ($antivirusProducts) {
    Write-Host "??  Software antivirus detectado:" -ForegroundColor Yellow
    foreach ($av in $antivirusProducts) {
        Write-Host "   ? $($av.displayName)" -ForegroundColor Gray
    }
    Write-Host "   ? Si tienes problemas, considera deshabilitar temporalmente el firewall del antivirus" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host "? No se detectó software antivirus adicional" -ForegroundColor Green
    Write-Host ""
}

# Resumen final
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host "  RESUMEN Y RECOMENDACIONES" -ForegroundColor Cyan
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""

if (-not $hasErrors -and $apiRunning) {
    Write-Host "?? EXCELENTE: Todo parece estar configurado correctamente!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? URLs para tu aplicación Android:" -ForegroundColor Cyan
    Write-Host "   ?? EMULADOR:" -ForegroundColor White
    Write-Host "   ?  http://10.0.2.2:5120/api/sla/solicitudes?meses=12" -ForegroundColor Green
    Write-Host "   ?" -ForegroundColor White
    Write-Host "   ?? DISPOSITIVO FÍSICO (misma WiFi):" -ForegroundColor White
    Write-Host "      http://$mainIp`:5120/api/sla/solicitudes?meses=12" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? PRÓXIMOS PASOS SI AÚN NO FUNCIONA:" -ForegroundColor Cyan
    Write-Host "   1. Reinicia el emulador de Android" -ForegroundColor Yellow
    Write-Host "   2. Limpia y reconstruye tu app Android" -ForegroundColor Yellow
    Write-Host "   3. Verifica los logs de Android Studio (Logcat)" -ForegroundColor Yellow
    Write-Host "   4. Prueba desde el navegador del emulador: http://10.0.2.2:5120/api/area" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host "??  SE ENCONTRARON PROBLEMAS QUE NECESITAN ATENCIÓN" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? ACCIONES REQUERIDAS:" -ForegroundColor Cyan
    
    if (-not $apiRunning) {
        Write-Host "   1. ? INICIA LA API (Visual Studio F5 con perfil 'http')" -ForegroundColor Yellow
    }
    
    if (-not $listeningOnAll) {
        Write-Host "   2. ? Asegúrate de que launchSettings.json tenga http://0.0.0.0:5120" -ForegroundColor Yellow
    }
    
    if (-not ($firewallRule -like "*ASP.NET Core API - Port 5120*")) {
        Write-Host "   3. ? Configura la regla de firewall (PowerShell como ADMINISTRADOR):" -ForegroundColor Yellow
        Write-Host "      netsh advfirewall firewall add rule name=`"ASP.NET Core API - Port 5120`" dir=in action=allow protocol=TCP localport=5120" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Host "   Después de resolver los problemas anteriores, ejecuta este script nuevamente." -ForegroundColor Gray
    Write-Host ""
}

Write-Host "?? CONSEJOS ADICIONALES:" -ForegroundColor Cyan
Write-Host "   • Mantén la ventana de la API abierta mientras pruebas" -ForegroundColor Gray
Write-Host "   • Usa el perfil 'http' no 'https' para desarrollo con Android" -ForegroundColor Gray
Write-Host "   • Si cambias de red WiFi, tu IP local puede cambiar" -ForegroundColor Gray
Write-Host "   • Para dispositivos físicos, PC y móvil deben estar en la MISMA red" -ForegroundColor Gray
Write-Host ""
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""
