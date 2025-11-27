# Script para verificar configuración HTTP vs HTTPS

Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host "  VERIFICACIÓN: CONFIGURACIÓN HTTP vs HTTPS PARA ANDROID" -ForegroundColor Cyan
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""

$hasIssues = $false

# 1. Verificar puertos escuchando
Write-Host "1??  VERIFICANDO PUERTOS EN ESCUCHA..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

$port5120 = netstat -ano | Select-String ":5120.*LISTENING"
$port7263 = netstat -ano | Select-String ":7263.*LISTENING"

if ($port5120) {
    Write-Host "? Puerto 5120 (HTTP) está ESCUCHANDO" -ForegroundColor Green
    $port5120 | ForEach-Object { Write-Host "   $($_.ToString().Trim())" -ForegroundColor Gray }
} else {
    Write-Host "? Puerto 5120 (HTTP) NO está escuchando" -ForegroundColor Red
    Write-Host "   ? La API no está corriendo o no está usando el perfil correcto" -ForegroundColor Yellow
    $hasIssues = $true
}

Write-Host ""

if ($port7263) {
    Write-Host "??  Puerto 7263 (HTTPS) está ESCUCHANDO" -ForegroundColor Yellow
    $port7263 | ForEach-Object { Write-Host "   $($_.ToString().Trim())" -ForegroundColor Gray }
    Write-Host "   ? ADVERTENCIA: Estás usando el perfil 'https' que NO funciona con Android" -ForegroundColor Red
    Write-Host "   ? SOLUCIÓN: Detén la API y usa el perfil 'http' en su lugar" -ForegroundColor Yellow
    $hasIssues = $true
} else {
    Write-Host "? Puerto 7263 (HTTPS) NO está escuchando - Correcto para desarrollo Android" -ForegroundColor Green
}

Write-Host ""

# 2. Probar conexión HTTP
Write-Host "2??  PROBANDO CONEXIÓN HTTP (localhost:5120)..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

if ($port5120) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5120/api/area" -Method GET -TimeoutSec 5 -ErrorAction Stop
        Write-Host "? HTTP funciona correctamente" -ForegroundColor Green
        Write-Host "   Status: $($response.StatusCode)" -ForegroundColor Gray
        Write-Host "   Content-Type: $($response.Headers.'Content-Type')" -ForegroundColor Gray
    } catch {
        if ($_.Exception.Message -like "*SSL*" -or $_.Exception.Message -like "*HTTPS*") {
            Write-Host "? ERROR: La API está redirigiendo a HTTPS" -ForegroundColor Red
            Write-Host "   ? Verifica que 'app.UseHttpsRedirection()' esté comentado en Program.cs" -ForegroundColor Yellow
            $hasIssues = $true
        } else {
            Write-Host "??  Error al conectar: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "??  OMITIDO - API no está corriendo en puerto 5120" -ForegroundColor Yellow
}

Write-Host ""

# 3. Intentar conexión HTTPS (debe fallar o no estar disponible)
Write-Host "3??  VERIFICANDO QUE HTTPS NO ESTÉ ACTIVO..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

if ($port7263) {
    Write-Host "? HTTPS (puerto 7263) ESTÁ ACTIVO" -ForegroundColor Red
    Write-Host "   ? Android NO puede conectarse a HTTPS con certificados autofirmados" -ForegroundColor Yellow
    Write-Host "   ? ACCIÓN REQUERIDA: Detén la API y reiníciala con el perfil 'http'" -ForegroundColor Yellow
    $hasIssues = $true
} else {
    Write-Host "? HTTPS NO está activo - Perfecto para desarrollo con Android" -ForegroundColor Green
}

Write-Host ""

# 4. Verificar launchSettings.json
Write-Host "4??  VERIFICANDO launchSettings.json..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

try {
    $launchSettings = Get-Content "Proyecto01.API\Properties\launchSettings.json" -Raw | ConvertFrom-Json
    
    Write-Host "Perfil 'http':" -ForegroundColor Cyan
    Write-Host "   applicationUrl: $($launchSettings.profiles.http.applicationUrl)" -ForegroundColor Gray
    
    if ($launchSettings.profiles.http.applicationUrl -like "*http://*:5120*" -and 
        $launchSettings.profiles.http.applicationUrl -notlike "*https://*") {
        Write-Host "   ? Configuración correcta para Android" -ForegroundColor Green
    } else {
        Write-Host "   ??  La configuración podría causar problemas" -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "Perfil 'https':" -ForegroundColor Cyan
    Write-Host "   applicationUrl: $($launchSettings.profiles.https.applicationUrl)" -ForegroundColor Gray
    Write-Host "   ??  NO usar este perfil para desarrollo con Android" -ForegroundColor Yellow
    
} catch {
    Write-Host "??  No se pudo leer launchSettings.json" -ForegroundColor Yellow
}

Write-Host ""

# 5. Verificar Program.cs
Write-Host "5??  VERIFICANDO Program.cs..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????????????????????????????????????????" -ForegroundColor DarkGray

try {
    $programCs = Get-Content "Proyecto01.API\Program.cs" -Raw
    
    if ($programCs -match "(?<!//\s*)app\.UseHttpsRedirection\(\);") {
        Write-Host "? 'app.UseHttpsRedirection()' está HABILITADO" -ForegroundColor Red
        Write-Host "   ? Esto forzará redirección a HTTPS" -ForegroundColor Yellow
        Write-Host "   ? SOLUCIÓN: Comenta la línea: // app.UseHttpsRedirection();" -ForegroundColor Yellow
        $hasIssues = $true
    } elseif ($programCs -match "//\s*app\.UseHttpsRedirection\(\);") {
        Write-Host "? 'app.UseHttpsRedirection()' está COMENTADO - Correcto" -ForegroundColor Green
    } else {
        Write-Host "??  No se encontró 'app.UseHttpsRedirection()' en el archivo" -ForegroundColor Yellow
    }
} catch {
    Write-Host "??  No se pudo leer Program.cs" -ForegroundColor Yellow
}

Write-Host ""

# Resumen
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host "  RESUMEN" -ForegroundColor Cyan
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""

if (-not $hasIssues -and $port5120 -and -not $port7263) {
    Write-Host "?? PERFECTO: Tu API está configurada correctamente para Android" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Usa estas URLs en tu aplicación Android:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   Para EMULADOR:" -ForegroundColor White
    Write-Host "   http://10.0.2.2:5120/api/sla/solicitudes?meses=12" -ForegroundColor Green
    Write-Host ""
    
    $localIps = Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
        $_.IPAddress -ne "127.0.0.1" -and $_.PrefixOrigin -ne "WellKnown"
    } | Select-Object -First 1 -ExpandProperty IPAddress
    
    if ($localIps) {
        Write-Host "   Para DISPOSITIVO FÍSICO (misma WiFi):" -ForegroundColor White
        Write-Host "   http://$localIps`:5120/api/sla/solicitudes?meses=12" -ForegroundColor Green
    }
    Write-Host ""
    
} else {
    Write-Host "??  PROBLEMAS DETECTADOS" -ForegroundColor Red
    Write-Host ""
    
    if (-not $port5120) {
        Write-Host "? La API NO está corriendo" -ForegroundColor Yellow
        Write-Host "   ? Inicia la API con Visual Studio usando el perfil 'http'" -ForegroundColor Gray
        Write-Host ""
    }
    
    if ($port7263) {
        Write-Host "? Estás usando el perfil 'https'" -ForegroundColor Yellow
        Write-Host "   ? Detén la API" -ForegroundColor Gray
        Write-Host "   ? En Visual Studio, selecciona el perfil 'http' (NO 'https')" -ForegroundColor Gray
        Write-Host "   ? Reinicia la API con F5" -ForegroundColor Gray
        Write-Host ""
    }
    
    if ($hasIssues) {
        Write-Host "? Hay problemas de configuración" -ForegroundColor Yellow
        Write-Host "   ? Revisa las secciones anteriores para más detalles" -ForegroundColor Gray
        Write-Host "   ? Consulta: SOLUCION_HTTPS_ANDROID.md" -ForegroundColor Gray
        Write-Host ""
    }
}

Write-Host "?? Documentación adicional:" -ForegroundColor Cyan
Write-Host "   • SOLUCION_HTTPS_ANDROID.md - Guía completa sobre HTTP vs HTTPS" -ForegroundColor Gray
Write-Host "   • INSTRUCCIONES_CONECTIVIDAD_ANDROID.md - Guía general de conectividad" -ForegroundColor Gray
Write-Host "   • DIAGNOSTICO_COMPLETO.ps1 - Diagnóstico completo del sistema" -ForegroundColor Gray
Write-Host ""
Write-Host "========================================================================================================" -ForegroundColor Cyan
Write-Host ""
