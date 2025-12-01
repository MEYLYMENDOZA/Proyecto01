# Script de Verificación Completa - Conectividad Android con API

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  DIAGNÓSTICO DE CONECTIVIDAD API" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar si la API está corriendo
Write-Host "1??  Verificando si la API está corriendo en el puerto 5120..." -ForegroundColor Yellow
$apiRunning = netstat -ano | Select-String ":5120"

if ($apiRunning) {
    Write-Host "? La API está CORRIENDO" -ForegroundColor Green
    Write-Host $apiRunning -ForegroundColor Gray
} else {
    Write-Host "? La API NO está corriendo" -ForegroundColor Red
    Write-Host "   ? Necesitas iniciar la API con: .\INICIAR_API.ps1" -ForegroundColor Yellow
}
Write-Host ""

# 2. Verificar regla de firewall
Write-Host "2??  Verificando regla de firewall..." -ForegroundColor Yellow
$firewallRule = netsh advfirewall firewall show rule name="ASP.NET Core API - Port 5120" 2>$null

if ($firewallRule -like "*ASP.NET Core API - Port 5120*") {
    Write-Host "? Regla de firewall configurada" -ForegroundColor Green
} else {
    Write-Host "? Regla de firewall NO configurada" -ForegroundColor Red
    Write-Host "   ? Ejecuta como ADMINISTRADOR:" -ForegroundColor Yellow
    Write-Host "   netsh advfirewall firewall add rule name=`"ASP.NET Core API - Port 5120`" dir=in action=allow protocol=TCP localport=5120" -ForegroundColor Cyan
}
Write-Host ""

# 3. Obtener IP local
Write-Host "3??  Obteniendo tu IP local..." -ForegroundColor Yellow
$ipAddresses = ipconfig | Select-String -Pattern "IPv4" | ForEach-Object { 
    $_.ToString().Split(':')[1].Trim() 
}

if ($ipAddresses) {
    Write-Host "?? Tus direcciones IP locales:" -ForegroundColor Green
    foreach ($ip in $ipAddresses) {
        Write-Host "   ? $ip" -ForegroundColor Cyan
        Write-Host "      Usa en Android (dispositivo físico): http://$ip`:5120/" -ForegroundColor Gray
    }
} else {
    Write-Host "??  No se pudieron obtener las IPs locales" -ForegroundColor Yellow
}
Write-Host ""

# 4. Probar conectividad local
Write-Host "4??  Probando conectividad al puerto 5120..." -ForegroundColor Yellow
$testConnection = Test-NetConnection -ComputerName localhost -Port 5120 -InformationLevel Quiet -WarningAction SilentlyContinue

if ($testConnection) {
    Write-Host "? Puerto 5120 es ACCESIBLE" -ForegroundColor Green
    
    # Intentar hacer una petición HTTP
    Write-Host ""
    Write-Host "5??  Probando endpoint /api/area..." -ForegroundColor Yellow
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5120/api/area" -Method GET -TimeoutSec 5 -ErrorAction Stop
        Write-Host "? API responde correctamente (Status: $($response.StatusCode))" -ForegroundColor Green
        Write-Host "   Respuesta:" -ForegroundColor Gray
        Write-Host $response.Content.Substring(0, [Math]::Min(200, $response.Content.Length)) -ForegroundColor Gray
    } catch {
        Write-Host "??  Error al hacer petición: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "? Puerto 5120 NO es accesible" -ForegroundColor Red
}
Write-Host ""

# Resumen
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  RESUMEN Y PRÓXIMOS PASOS" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

if (-not $apiRunning) {
    Write-Host "?? ACCIÓN REQUERIDA:" -ForegroundColor Red
    Write-Host "   1. Inicia la API ejecutando: .\INICIAR_API.ps1" -ForegroundColor Yellow
    Write-Host "   2. O desde Visual Studio: Presiona F5 con perfil 'http'" -ForegroundColor Yellow
    Write-Host ""
}

if (-not ($firewallRule -like "*ASP.NET Core API - Port 5120*")) {
    Write-Host "?? ACCIÓN REQUERIDA (como ADMINISTRADOR):" -ForegroundColor Red
    Write-Host "   netsh advfirewall firewall add rule name=`"ASP.NET Core API - Port 5120`" dir=in action=allow protocol=TCP localport=5120" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host "?? URLs para tu app Android:" -ForegroundColor Cyan
Write-Host "   Emulador: http://10.0.2.2:5120/" -ForegroundColor Green
if ($ipAddresses -and $ipAddresses.Count -gt 0) {
    Write-Host "   Dispositivo físico: http://$($ipAddresses[0]):5120/" -ForegroundColor Green
}
Write-Host ""
Write-Host "? Para más información, consulta: INSTRUCCIONES_CONECTIVIDAD_ANDROID.md" -ForegroundColor Cyan
Write-Host ""
