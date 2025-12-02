# =====================================================
# Script de Prueba - API de Usuario
# PowerShell para Windows
# =====================================================

$baseUrl = "http://localhost:5120/api/User"

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "  Proyecto01 - API Usuario - Testing Script" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# =====================================================
# Función Helper para hacer requests
# =====================================================
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [object]$Body = $null,
        [string]$Description
    )
    
    Write-Host "[$Method] $Description" -ForegroundColor Yellow
    Write-Host "URL: $Endpoint" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = $Endpoint
            Method = $Method
            ContentType = "application/json"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
            Write-Host "Body:" -ForegroundColor Gray
            Write-Host ($Body | ConvertTo-Json -Depth 10) -ForegroundColor DarkGray
        }
        
        $response = Invoke-RestMethod @params
        
        Write-Host "? SUCCESS" -ForegroundColor Green
        Write-Host "Response:" -ForegroundColor Gray
        Write-Host ($response | ConvertTo-Json -Depth 10) -ForegroundColor Green
    }
    catch {
        Write-Host "? ERROR" -ForegroundColor Red
        Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
        Write-Host "Message: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "---------------------------------------------" -ForegroundColor DarkGray
    Write-Host ""
}

# =====================================================
# Menú Principal
# =====================================================
function Show-Menu {
    Write-Host "Selecciona una opción:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Crear Usuario Administrador" -ForegroundColor White
    Write-Host "2. Crear Usuario Básico" -ForegroundColor White
    Write-Host "3. Iniciar Sesión" -ForegroundColor White
    Write-Host "4. Obtener Todos los Usuarios" -ForegroundColor White
    Write-Host "5. Obtener Usuario por ID" -ForegroundColor White
    Write-Host "6. Actualizar Usuario" -ForegroundColor White
    Write-Host "7. Eliminar Usuario" -ForegroundColor White
    Write-Host "8. Ejecutar Flujo Completo de Prueba" -ForegroundColor Yellow
    Write-Host "9. Salir" -ForegroundColor Red
    Write-Host ""
}

# =====================================================
# Tests Individuales
# =====================================================

function Test-CreateAdminUser {
    $body = @{
        username = "admin"
        correo = "admin@proyecto01.com"
        password = "Admin123456"
        idRolSistema = 1
        idEstadoUsuario = 1
    }
    
    Invoke-ApiRequest -Method "POST" -Endpoint $baseUrl -Body $body -Description "Crear Usuario Administrador"
}

function Test-CreateBasicUser {
    $body = @{
        username = "usuario1"
        correo = "usuario1@proyecto01.com"
        password = "Password123"
        idRolSistema = 2
        idEstadoUsuario = 1
    }
    
    Invoke-ApiRequest -Method "POST" -Endpoint $baseUrl -Body $body -Description "Crear Usuario Básico"
}

function Test-SignIn {
    $correo = Read-Host "Ingresa el correo"
    $password = Read-Host "Ingresa la contraseña" -AsSecureString
    $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)
    )
    
    $body = @{
        correo = $correo
        password = $passwordPlain
    }
    
    Invoke-ApiRequest -Method "POST" -Endpoint "$baseUrl/SignIn" -Body $body -Description "Iniciar Sesión"
}

function Test-GetAllUsers {
    Invoke-ApiRequest -Method "GET" -Endpoint $baseUrl -Description "Obtener Todos los Usuarios"
}

function Test-GetUserById {
    $id = Read-Host "Ingresa el ID del usuario"
    Invoke-ApiRequest -Method "GET" -Endpoint "$baseUrl/$id" -Description "Obtener Usuario por ID"
}

function Test-UpdateUser {
    $id = Read-Host "Ingresa el ID del usuario a actualizar"
    
    $body = @{
        idUsuario = [int]$id
        username = "usuario_updated"
        correo = "usuario_updated@proyecto01.com"
        password = "NewPassword123"
        idRolSistema = 2
        idEstadoUsuario = 1
    }
    
    Invoke-ApiRequest -Method "PUT" -Endpoint "$baseUrl/$id" -Body $body -Description "Actualizar Usuario"
}

function Test-DeleteUser {
    $id = Read-Host "Ingresa el ID del usuario a eliminar"
    Invoke-ApiRequest -Method "DELETE" -Endpoint "$baseUrl/$id" -Description "Eliminar Usuario"
}

# =====================================================
# Flujo Completo de Prueba
# =====================================================
function Test-CompleteFlow {
    Write-Host "=============================================" -ForegroundColor Magenta
    Write-Host "  FLUJO COMPLETO DE PRUEBA" -ForegroundColor Magenta
    Write-Host "=============================================" -ForegroundColor Magenta
    Write-Host ""
    
    # 1. Crear Usuario
    Write-Host "PASO 1: Crear Usuario" -ForegroundColor Cyan
    $bodyCreate = @{
        username = "testuser_$(Get-Date -Format 'yyyyMMddHHmmss')"
        correo = "testuser_$(Get-Date -Format 'yyyyMMddHHmmss')@proyecto01.com"
        password = "Password123"
        idRolSistema = 2
        idEstadoUsuario = 1
    }
    
    Invoke-ApiRequest -Method "POST" -Endpoint $baseUrl -Body $bodyCreate -Description "Crear Usuario de Prueba"
    
    Start-Sleep -Seconds 2
    
    # 2. Login
    Write-Host "PASO 2: Iniciar Sesión" -ForegroundColor Cyan
    $bodyLogin = @{
        correo = $bodyCreate.correo
        password = $bodyCreate.password
    }
    
    Invoke-ApiRequest -Method "POST" -Endpoint "$baseUrl/SignIn" -Body $bodyLogin -Description "Login con Usuario Creado"
    
    Start-Sleep -Seconds 2
    
    # 3. Obtener Todos
    Write-Host "PASO 3: Obtener Todos los Usuarios" -ForegroundColor Cyan
    Invoke-ApiRequest -Method "GET" -Endpoint $baseUrl -Description "Listar Usuarios"
    
    Start-Sleep -Seconds 2
    
    Write-Host "=============================================" -ForegroundColor Magenta
    Write-Host "  FLUJO COMPLETO TERMINADO" -ForegroundColor Magenta
    Write-Host "=============================================" -ForegroundColor Magenta
    Write-Host ""
}

# =====================================================
# Loop Principal
# =====================================================
do {
    Show-Menu
    $option = Read-Host "Opción"
    Write-Host ""
    
    switch ($option) {
        "1" { Test-CreateAdminUser }
        "2" { Test-CreateBasicUser }
        "3" { Test-SignIn }
        "4" { Test-GetAllUsers }
        "5" { Test-GetUserById }
        "6" { Test-UpdateUser }
        "7" { Test-DeleteUser }
        "8" { Test-CompleteFlow }
        "9" { 
            Write-Host "Saliendo..." -ForegroundColor Yellow
            break
        }
        default {
            Write-Host "Opción inválida" -ForegroundColor Red
            Write-Host ""
        }
    }
    
    if ($option -ne "9") {
        Write-Host "Presiona Enter para continuar..." -ForegroundColor DarkGray
        Read-Host
        Clear-Host
    }
    
} while ($option -ne "9")

Write-Host "¡Hasta luego! ??" -ForegroundColor Green
