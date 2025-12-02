# ?? **DIAGNÓSTICO DETALLADO - API de Usuarios**

## ?? **Paso 1: Verificar la Cadena de Conexión**

### Tu cadena de conexión actual:
```
Server=.\SQLEXPRESS;Database=Proyecto01;Integrated Security=True;TrustServerCertificate=True
```

### ? Puntos a verificar:

1. **¿SQL Server está corriendo?**
   ```powershell
   # En PowerShell (como Administrador):
   Get-Service MSSQLSERVER
   # Debe mostrar: Status: Running
   ```

2. **¿La base de datos existe?**
   ```sql
   SELECT name FROM sys.databases WHERE name = 'Proyecto01';
   ```

3. **¿Las tablas existen?**
   ```sql
   USE Proyecto01;
   SELECT COUNT(*) as TotalTablas FROM information_schema.TABLES;
   ```

---

## ?? **Paso 2: Verificar Datos Base en BD**

### Ejecuta estos comandos SQL:

```sql
USE Proyecto01;

-- Verificar tabla usuario
SELECT COUNT(*) as CantidadUsuarios FROM usuario;

-- Verificar tabla roles_sistema
SELECT * FROM roles_sistema;

-- Verificar tabla estado_usuario_catalogo
SELECT * FROM estado_usuario_catalogo;

-- Mostrar estructura de usuario
EXEC sp_help 'usuario';
```

---

## ?? **Paso 3: Prueba de Diagnóstico con Logging**

### A. Ejecutar la aplicación:

1. Abre Visual Studio
2. Presiona **F5** o haz clic en "Iniciar depuración"
3. **Observa la ventana de salida (Output Window)** - Busca los logs

### B. Qué buscar en los logs:

```
? Cadena de conexión configurada
? Ambiente: Desarrollo
? API iniciada correctamente en puerto 5120
```

### C. Hacer solicitud GET:

```
GET http://localhost:5120/api/User
```

### D. Revisar los logs en orden:

```
[INFO] UsuarioRepository.GetAll() - Iniciando consulta
[INFO] UsuarioRepository.GetAll() - Conectando a Usuarios DbSet
[INFO] UsuarioRepository.GetAll() - Consulta exitosa, X registros encontrados
```

---

## ?? **Paso 4: Identificar el Problema**

### Escenario 1: **Error de Conexión**

**Logs esperados:**
```
[ERROR] UsuarioRepository.GetAll() - Error al obtener usuarios
[ERROR] SqlException: A network-related or instance-specific error...
```

**Solución:**
```powershell
# Verificar SQL Server
sqlcmd -S .\SQLEXPRESS -E -Q "SELECT @@version"

# Si falla, reinicia SQL Server:
Get-Service MSSQLSERVER | Restart-Service -Force
```

---

### Escenario 2: **Base de Datos no Existe**

**Logs esperados:**
```
[ERROR] SqlException: Cannot open database "Proyecto01"
```

**Solución:**
```sql
-- Crear la base de datos
CREATE DATABASE Proyecto01;

-- Luego ejecutar el script de creación de tablas
```

---

### Escenario 3: **Tabla no Existe**

**Logs esperados:**
```
[ERROR] SqlException: Invalid object name 'usuario'
```

**Solución:**
```sql
USE Proyecto01;

-- Ejecutar el script de creación de tablas:
-- Ubicación: Tu archivo de schema SQL
```

---

### Escenario 4: **No hay roles ni estados**

**Síntomas:** El POST falla con error FK constraint

**Solución:**
```sql
USE Proyecto01;

INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
VALUES 
('ADMIN', 'Administrador', 'Rol con todos los permisos', 1),
('USER', 'Usuario', 'Rol básico', 1),
('GESTOR', 'Gestor', 'Rol de gestor', 1);

INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
VALUES 
('ACTIVO', 'Usuario activo', 1),
('INACTIVO', 'Usuario inactivo', 1),
('BLOQUEADO', 'Usuario bloqueado', 1);
```

---

## ?? **Paso 5: Prueba Aislada de Consulta**

### Modificar temporalmente GetAll() para diagnóstico:

**En `UsuarioRepository.cs`, línea 24:**

```csharp
public async Task<IEnumerable<UsuarioResponseDTO>> GetAll()
{
    try
    {
        _logger.LogInformation("GetAll() - Test 1: Retornando lista vacía");
        
        // PRUEBA 1: Verificar si el problema es la consulta
        return new List<UsuarioResponseDTO>();
        
        // Si funciona arriba, el problema está abajo:
        // Descomenta para Test 2:
        
        /*
        _logger.LogInformation("GetAll() - Test 2: Consultando Usuarios DbSet");
        
        var usuarios = await _context.Usuarios
            .Select(u => new UsuarioResponseDTO
            {
                IdUsuario = u.IdUsuario,
                Username = u.Username,
                Correo = u.Correo,
                IdRolSistema = u.IdRolSistema,
                IdEstadoUsuario = u.IdEstadoUsuario,
                CreadoEn = u.CreadoEn
            })
            .ToListAsync();

        _logger.LogInformation($"GetAll() - Test 2: {usuarios.Count} registros encontrados");
        return usuarios;
        */
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "GetAll() - Error");
        throw;
    }
}
```

**Pruebas:**

1. **Test 1 (lista vacía):**
   ```
   GET http://localhost:5120/api/User
   ? Debe retornar: []
   ```

2. **Test 2 (consulta real):**
   - Comenta Test 1
   - Descomenta Test 2
   - Ejecuta de nuevo
   - Si falla aquí, el problema es la consulta a BD

---

## ?? **Paso 6: Verificar Estructura de Datos**

### Validar que las columnas coincidan:

```sql
USE Proyecto01;

-- Ver estructura de tabla usuario
EXEC sp_help 'usuario';

-- Ver estructura de tabla estado_usuario_catalogo
EXEC sp_help 'estado_usuario_catalogo';

-- Ver estructura de tabla roles_sistema
EXEC sp_help 'roles_sistema';
```

### Comparar con entidades C#:

**Tabla `usuario` debe tener:**
- `id_usuario` ? `int`
- `username` ? `varchar`
- `correo` ? `varchar`
- `password_hash` ? `varchar`
- `id_rol_sistema` ? `int`
- `id_estado_usuario` ? `int`
- `creado_en` ? `datetime2`

---

## ?? **Paso 7: Prueba de POST (Crear Usuario)**

### Request:
```json
POST http://localhost:5120/api/User

{
  "username": "testuser",
  "correo": "test@example.com",
  "password": "Password123",
  "idRolSistema": 1,
  "idEstadoUsuario": 1
}
```

### Logs esperados en caso de éxito:
```
[INFO] UsuarioRepository.ExistsByUsername('testuser') - Username existe: False
[INFO] UsuarioRepository.ExistsByCorreo('test@example.com') - Correo existe: False
[INFO] UsuarioRepository.Insert() - Creando usuario: testuser
[INFO] UsuarioRepository.Insert() - Usuario creado exitosamente (ID: 1)
```

### Logs esperados en caso de error:
```
[ERROR] UsuarioRepository.Insert() - Error al crear usuario: testuser
[ERROR] SqlException: The INSERT statement conflicted with the FOREIGN KEY constraint...
```

---

## ?? **Paso 8: Checklist de Verificación**

Antes de continuar, verifica:

- [ ] SQL Server está corriendo
- [ ] Database `Proyecto01` existe
- [ ] Las tablas de catálogo existen
- [ ] Los datos base (roles, estados) están insertados
- [ ] La cadena de conexión es correcta
- [ ] La API inicia sin errores
- [ ] El endpoint GET devuelve una respuesta
- [ ] El endpoint POST crea usuarios correctamente

---

## ?? **Información a Recopilar si Hay Error**

Si algo falla, recopila esto:

1. **Mensaje de error completo** de los logs
2. **Stack trace** si lo hay
3. **Nombre de la tabla/columna** donde falla
4. **Valores** que intentas insertar
5. **Estado de SQL Server** (running/stopped)

---

## ?? **Reinicio Limpio**

Si todo falla, intenta esto:

```powershell
# 1. Detener la API (Ctrl+Shift+F5 en Visual Studio)

# 2. Reiniciar SQL Server
Get-Service MSSQLSERVER | Restart-Service -Force

# 3. Limpiar base de datos (OPCIÓN: Si quieres empezar limpio)
# - Eliminar BD: DROP DATABASE Proyecto01
# - Crear nueva: CREATE DATABASE Proyecto01
# - Restaurar tablas: Ejecutar script SQL

# 4. Reiniciar Visual Studio
# - Ctrl+Shift+Esc (abrir Task Manager)
# - Terminar devenv.exe
# - Abrir Visual Studio de nuevo

# 5. Limpiar build
# - Ctrl+Alt+L (Solution Explorer)
# - Click derecho en Solution
# - Clean Solution
# - Build Solution

# 6. Iniciar debug (F5)
```

---

## ? **Resumen**

Esta guía te ayuda a:
1. ? Identificar si el problema es conexión, datos o código
2. ? Aislar la consulta problemática
3. ? Ver logs detallados de cada operación
4. ? Resolver errores sistemáticamente

**Sigue los pasos en orden y reporta qué test falla.**
