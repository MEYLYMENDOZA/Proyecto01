# ??? Solución al Error de Eliminación de Usuario

## ? **Problema Original**

Cuando intentabas eliminar un usuario, obtenías este error:

```
The DELETE statement conflicted with the REFERENCE constraint "FK__config_sl__cread__51300E55". 
The conflict occurred in database "Proyecto01", table "dbo.config_sla", column 'creado_por'.
```

### **Causa del Error**

Tu base de datos tiene **restricciones de integridad referencial**. Los usuarios que han creado registros en otras tablas (`config_sla`, `solicitud`, `reporte`) NO pueden ser eliminados físicamente porque otros datos dependen de ellos.

Tablas que referencian a `usuario`:
- ? `config_sla.creado_por`
- ? `config_sla.actualizado_por`
- ? `solicitud.creado_por`
- ? `solicitud.actualizado_por`
- ? `reporte.generado_por`
- ? `personal.id_usuario`

---

## ? **Solución Implementada: Soft Delete**

En lugar de eliminar físicamente el usuario de la base de datos, ahora el sistema lo **marca como INACTIVO**.

### **¿Qué es Soft Delete?**

**Soft Delete** (eliminación lógica) es una práctica común en desarrollo donde:
- ? NO se elimina el registro físicamente de la base de datos
- ? Se cambia su estado a "INACTIVO", "ELIMINADO" o "BLOQUEADO"
- ? Se mantiene la integridad referencial
- ? Se preserva el historial de auditoría

---

## ?? **Cambios Implementados**

### **1. UsuarioRepository.cs - Método Delete (Soft Delete)**

```csharp
public async Task<bool> Delete(int id)
{
    var usuario = await _context.Usuarios.FindAsync(id);
    if (usuario == null) return false;

    // Buscar el ID del estado "INACTIVO" o "BLOQUEADO"
    var estadoInactivo = await _context.EstadosUsuario
        .Where(e => e.Codigo == "INACTIVO" || e.Codigo == "BLOQUEADO")
        .Select(e => e.IdEstadoUsuario)
        .FirstOrDefaultAsync();

    if (estadoInactivo == 0)
    {
        estadoInactivo = 2; // Default: INACTIVO
    }

    // Soft delete: cambiar estado a INACTIVO
    usuario.IdEstadoUsuario = estadoInactivo;
    usuario.ActualizadoEn = DateTime.UtcNow;

    _context.Usuarios.Update(usuario);
    await _context.SaveChangesAsync();
    return true;
}
```

### **2. Método HardDelete (Solo para casos especiales)**

```csharp
public async Task<bool> HardDelete(int id)
{
    var usuario = await _context.Usuarios.FindAsync(id);
    if (usuario == null) return false;

    // Verificar si tiene registros relacionados
    var tieneConfigSla = await _context.ConfigSlas
        .AnyAsync(c => c.CreadoPor == id || c.ActualizadoPor == id);

    var tieneSolicitudes = await _context.Solicitudes
        .AnyAsync(s => s.CreadoPor == id || s.ActualizadoPor == id);

    var tieneReportes = await _context.Reportes
        .AnyAsync(r => r.GeneradoPor == id);

    if (tieneConfigSla || tieneSolicitudes || tieneReportes)
    {
        return false; // No se puede eliminar
    }

    _context.Usuarios.Remove(usuario);
    await _context.SaveChangesAsync();
    return true;
}
```

### **3. UsuarioService.cs - Validación en SignIn**

Ahora el login verifica que el usuario esté **ACTIVO**:

```csharp
// Verificar que el usuario esté activo (IdEstadoUsuario = 1)
if (usuario.IdEstadoUsuario != 1)
    return null; // Usuario inactivo o bloqueado
```

### **4. UsuarioResponseDTO.cs - Campo IdEstadoUsuario**

```csharp
public int IdEstadoUsuario { get; set; }
```

Ahora puedes ver el estado del usuario en las respuestas.

---

## ?? **Cómo Funciona Ahora**

### **Antes (Eliminación Física - ERROR):**
```
DELETE FROM usuario WHERE id_usuario = 1;
? ERROR: FK constraint violation
```

### **Ahora (Soft Delete - FUNCIONA):**
```
UPDATE usuario 
SET id_estado_usuario = 2, actualizado_en = GETDATE()
WHERE id_usuario = 1;
? SUCCESS: Usuario marcado como INACTIVO
```

---

## ?? **Prueba del Endpoint DELETE**

### **Request:**
```
DELETE http://localhost:5120/api/User/1
```

### **Response (200 OK):**
```json
{
  "message": "Usuario desactivado exitosamente. El usuario ha sido marcado como inactivo.",
  "id": 1,
  "note": "Los datos del usuario se mantienen por integridad referencial."
}
```

### **Verificación en Base de Datos:**
```sql
SELECT id_usuario, username, correo, id_estado_usuario, actualizado_en
FROM usuario
WHERE id_usuario = 1;
```

**Resultado:**
```
id_usuario | username | correo              | id_estado_usuario | actualizado_en
-----------|----------|---------------------|-------------------|------------------
1          | admin    | admin@proyecto01.com| 2                 | 2024-01-15 15:30:00
```

---

## ?? **Validación de Login con Usuarios Inactivos**

Si intentas hacer login con un usuario desactivado:

### **Request:**
```json
POST http://localhost:5120/api/User/SignIn

{
  "correo": "admin@proyecto01.com",
  "password": "Admin123456"
}
```

### **Response (401 Unauthorized):**
```
Correo o contraseña incorrectos, o usuario inactivo.
```

---

## ?? **Estados de Usuario**

Según tu script de inicialización:

| ID | Código | Descripción |
|----|--------|-------------|
| 1  | ACTIVO | Usuario activo en el sistema |
| 2  | INACTIVO | Usuario inactivo temporalmente |
| 3  | BLOQUEADO | Usuario bloqueado por seguridad |

---

## ?? **Ventajas del Soft Delete**

? **Mantiene integridad referencial** - No rompe relaciones FK  
? **Preserva historial** - Sabes quién creó cada registro  
? **Auditoría completa** - Trazabilidad de acciones  
? **Reversible** - Puedes reactivar usuarios  
? **Cumple normativas** - GDPR, LOPD permiten mantener datos con consentimiento  

---

## ?? **Cómo Reactivar un Usuario**

Si necesitas reactivar un usuario desactivado:

### **Opción 1: Mediante PUT**
```json
PUT http://localhost:5120/api/User/1

{
  "idUsuario": 1,
  "username": "admin",
  "correo": "admin@proyecto01.com",
  "idRolSistema": 1,
  "idEstadoUsuario": 1  // Cambiar a ACTIVO
}
```

### **Opción 2: Mediante SQL directo**
```sql
UPDATE usuario 
SET id_estado_usuario = 1, actualizado_en = GETDATE()
WHERE id_usuario = 1;
```

---

## ?? **Cuándo Usar Hard Delete**

El método `HardDelete` SOLO debe usarse cuando:

1. ? El usuario NO tiene registros relacionados
2. ? Es un usuario de prueba recién creado
3. ? Cumples con regulaciones de privacidad (derecho al olvido)
4. ? Has limpiado todas las referencias manualmente

**?? IMPORTANTE:** `HardDelete` NO está expuesto en el controlador por seguridad.

---

## ?? **Resumen de Endpoints**

| Endpoint | Método | Acción | Estado Usuario |
|----------|--------|--------|----------------|
| `/api/User` | POST | Crear | ACTIVO (1) |
| `/api/User/SignIn` | POST | Login | Solo ACTIVO (1) |
| `/api/User/{id}` | GET | Consultar | Cualquiera |
| `/api/User` | GET | Listar todos | Todos (incluso inactivos) |
| `/api/User/{id}` | PUT | Actualizar | Puede cambiar estado |
| `/api/User/{id}` | DELETE | Soft Delete | Cambia a INACTIVO (2) |

---

## ? **Estado Final**

Tu API de Usuario ahora funciona correctamente con:

? **Soft Delete implementado** - No más errores FK  
? **Validación de estado en login** - Solo usuarios activos  
? **Preservación de integridad** - Datos históricos seguros  
? **Respuestas claras** - Mensajes informativos  
? **Auditoría completa** - Campo `actualizado_en` actualizado  

---

## ?? **¡Problema Resuelto!**

Ahora puedes "eliminar" usuarios sin problemas. El sistema:
1. Marca al usuario como INACTIVO
2. Mantiene todos los registros históricos
3. Previene que usuarios inactivos inicien sesión
4. Permite reactivar usuarios si es necesario

**No necesitas reiniciar la aplicación.** Los cambios ya están aplicados. Solo detén el debugger y vuelve a ejecutar la aplicación.
