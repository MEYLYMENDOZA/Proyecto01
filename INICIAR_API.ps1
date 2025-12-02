# Script para iniciar la API de Proyecto01
# Este script inicia la API en el puerto 5120

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  INICIANDO API - Proyecto01" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Cambiar al directorio de la API
$apiPath = "D:\REPOS\Proyecto movil backend\Proyecto01.API"
Set-Location $apiPath

Write-Host "?? Directorio: $apiPath" -ForegroundColor Yellow
Write-Host ""

# Verificar si el puerto 5120 ya está en uso
$portInUse = netstat -ano | Select-String ":5120"
if ($portInUse) {
    Write-Host "??  ADVERTENCIA: El puerto 5120 ya está en uso:" -ForegroundColor Yellow
    Write-Host $portInUse -ForegroundColor Gray
    Write-Host ""
    $continue = Read-Host "¿Deseas continuar de todos modos? (S/N)"
    if ($continue -ne "S" -and $continue -ne "s") {
        Write-Host "? Operación cancelada." -ForegroundColor Red
        exit
    }
}

Write-Host "?? Iniciando API en http://localhost:5120 y http://0.0.0.0:5120..." -ForegroundColor Green
Write-Host ""
Write-Host "?? Logs de la aplicación:" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Iniciar la API con el perfil http
dotnet run --launch-profile http
