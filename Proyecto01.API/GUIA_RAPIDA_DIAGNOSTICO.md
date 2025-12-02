# ?? **GUÍA RÁPIDA DE DIAGNÓSTICO Y CORRECCIÓN**

## ? **Acción Inmediata**

### **1. Detén la aplicación**
- Presiona **Shift + F5** en Visual Studio para detener el debugger

### **2. Limpia y reconstruye**
- **Ctrl + Alt + L** (abrir Solution Explorer)
- Click derecho en la solución ? **Clean Solution**
- Click derecho en la solución ? **Build Solution**
- Espera a que termine

### **3. Reinicia la aplicación**
- Presiona **F5** para iniciar con debugger

### **4. Observa los logs de inicio**
Busca estos mensajes en la **Ventana de Salida (Output Window)**:
```
? Cadena de conexión configurada
? Ambiente: Desarrollo
? API iniciada correctamente en puerto 5120
```

---

## ?? **Prueba GET - Obtener Todos los Usuarios**

### En Postman o cURL:

```
GET http://localhost:5120/api/User
```

### Logs que verás en Visual Studio:

```
[INFO] UsuarioRepository.GetAll() - Iniciando consulta
[INFO] UsuarioRepository.GetAll() - Conectando a Usuarios DbSet
[INFO] UsuarioRepository.GetAll() - Consulta exitosa, X registros encontrados
```

### Posibles respuestas:

**? Éxito (200 OK):**
```json
[]  // Lista vacía si no hay usuarios, o con usuarios
```

**? Error - Connection:**
```
[ERROR] SqlException: A network-related or instance-specific error occurred...
```

**? Error - Tabla no existe:**
```
[ERROR] SqlException: Invalid object name 'usuario'
```

---

## ??? **Soluciones Rápidas por Tipo de Error**

### **Error 1: Connection Timeout**

```powershell
# Verificar si SQL Server está corriendo:
Get-Service MSSQLSERVER

# Si no está corriendo:
Get-Service MSSQLSERVER | Start-Service

# Si está corriendo pero falla:
Get-Service MSSQLSERVER | Restart-Service -Force
```

---

### **Error 2: Database "Proyecto01" not found**

```sql
-- Conectar a SQL Server como admin
-- En SQL Server Management Studio:

CREATE DATABASE Proyecto01;
```

---

### **Error 3: Invalid object name 'usuario'**

```sql
USE Proyecto01;

-- Ejecutar el script completo de creación de tablas
-- (El que tiene toda la estructura de tu BD)
```

---

### **Error 4: FK constraint (al crear usuario)**

```sql
USE Proyecto01;

-- Verificar que existan los roles y estados:
SELECT * FROM roles_sistema;
SELECT * FROM estado_usuario_catalogo;

-- Si están vacíos, insertar datos:
INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
VALUES 
('ADMIN', 'Administrador', 'Rol administrativo', 1),
('USER', 'Usuario', 'Rol básico', 1),
('GESTOR', 'Gestor', 'Rol de gestor', 1);

INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
VALUES 
('ACTIVO', 'Estado activo', 1),
('INACTIVO', 'Estado inactivo', 1),
('BLOQUEADO', 'Estado bloqueado', 1);
```

---

## ?? **Prueba Completa - POST (Crear Usuario)**

### Request:

```
POST http://localhost:5120/api/User
Content-Type: application/json

{
  "username": "admin",
  "correo": "admin@proyecto01.com",
  "password": "Admin123456",
  "idRolSistema": 1,
  "idEstadoUsuario": 1
}
```

### Logs esperados:

```
[INFO] UsuarioRepository.ExistsByUsername('admin') - Username existe: False
[INFO] UsuarioRepository.ExistsByCorreo('admin@proyecto01.com') - Correo existe: False
[INFO] UsuarioRepository.Insert() - Creando usuario: admin
[INFO] UsuarioRepository.Insert() - Usuario creado exitosamente (ID: 1)
```

### Response esperado (201 Created):

```json
{
  "id": 1
}
```

---

## ?? **Prueba - POST Login**

### Request:

```
POST http://localhost:5120/api/User/SignIn
Content-Type: application/json

{
  "correo": "admin@proyecto01.com",
  "password": "Admin123456"
}
```

### Logs esperados:

```
[INFO] UsuarioRepository.GetByCorreo('admin@proyecto01.com') - Iniciando consulta
[INFO] UsuarioRepository.GetByCorreo('admin@proyecto01.com') - Usuario encontrado (ID: 1)
```

### Response esperado (200 OK):

```json
{
  "idUsuario": 1,
  "username": "admin",
  "correo": "admin@proyecto01.com",
  "passwordHash": null,
  "idRolSistema": 1,
  "idEstadoUsuario": 1,
  "creadoEn": "2024-01-15T15:30:00Z"
}
```

---

## ?? **Flujo de Debugging Paso a Paso**

### **1. Verifica conexión a BD**

```
GET http://localhost:5120/api/User
```

Si devuelve `[]` ? ? **Conexión OK**

Si devuelve error ? ? **Verifica SQL Server y Database**

### **2. Verifica datos base**

```
POST http://localhost:5120/api/User

{
  "username": "test",
  "correo": "test@example.com",
  "password": "Password123",
  "idRolSistema": 1,
  "idEstadoUsuario": 1
}
```

Si retorna `201 Created` ? ? **Datos base OK**

Si retorna error FK ? ? **Falta insertar roles/estados**

### **3. Verifica login**

```
POST http://localhost:5120/api/User/SignIn

{
  "correo": "test@example.com",
  "password": "Password123"
}
```

Si retorna usuario ? ? **Todo OK**

Si retorna `401` ? ? **Verifica credenciales**

---

## ?? **Checklist Final**

Antes de reportar un problema, verifica:

- [ ] SQL Server está corriendo (`Get-Service MSSQLSERVER`)
- [ ] Database `Proyecto01` existe
- [ ] Las tablas existen en la BD
- [ ] Los roles están insertados (`SELECT * FROM roles_sistema`)
- [ ] Los estados están insertados (`SELECT * FROM estado_usuario_catalogo`)
- [ ] La cadena de conexión es correcta en `appsettings.json`
- [ ] La API inicia sin errores
- [ ] `GET /api/User` devuelve respuesta
- [ ] `POST /api/User` crea usuario correctamente

---

## ?? **Si Todo Falla - Reinicio Limpio Completo**

```powershell
# 1. Detener Visual Studio completamente
Get-Process devenv | Stop-Process -Force

# 2. Reiniciar SQL Server
Get-Service MSSQLSERVER | Restart-Service -Force

# 3. Esperar 5 segundos
Start-Sleep -Seconds 5

# 4. Abrir Visual Studio
# Abrir el proyecto

# 5. En Visual Studio:
# - Ctrl + Shift + B (Rebuild Solution)
# - F5 (Start Debugging)
```

---

## ?? **Información a Recopilar si hay Error**

Si algo falla, recopila:

1. **Mensaje de error completo**
2. **Stack trace completo**
3. **Logs de Visual Studio** (Output Window)
4. **Comando SQL que fallaba** (si aplica)
5. **Versión de SQL Server** (`SELECT @@version`)

---

## ? **Estado Final Esperado**

Después de seguir esta guía, deberías tener:

? API corriendo en `http://localhost:5120`  
? Endpoint GET `/api/User` devolviendo usuarios  
? Endpoint POST `/api/User` creando usuarios  
? Endpoint POST `/api/User/SignIn` permitiendo login  
? Endpoint PUT `/api/User/{id}` actualizando usuarios  
? Endpoint DELETE `/api/User/{id}` desactivando usuarios  
? Todos los logs mostrándose en Visual Studio  

---

**¡Si llegas hasta aquí, tu API está completamente funcional! ??**
