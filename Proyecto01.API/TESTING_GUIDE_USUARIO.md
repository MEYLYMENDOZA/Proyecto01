# ?? Guía de Pruebas - API de Usuario

## ?? Información General
- **Base URL**: `http://localhost:5120`
- **Controlador**: `/api/User`

---

## ? 1. POST - Crear Usuario (SignUp)

### Endpoint
```
POST http://localhost:5120/api/User
Content-Type: application/json
```

### Body (JSON)
```json
{
  "username": "testuser",
  "correo": "testuser@example.com",
  "password": "Password123",
  "idRolSistema": 1,
  "idEstadoUsuario": 1
}
```

### Respuestas Esperadas

**? Éxito (201 Created):**
```json
{
  "id": 1
}
```

**? Error - Validación (400 Bad Request):**
```json
{
  "message": "El rol de sistema es requerido."
}
```

**? Error - Usuario duplicado (409 Conflict):**
```json
{
  "message": "El username ya existe."
}
```

**? Error - Correo duplicado (409 Conflict):**
```json
{
  "message": "El correo ya está registrado."
}
```

---

## ? 2. POST - Iniciar Sesión (SignIn)

### Endpoint
```
POST http://localhost:5120/api/User/SignIn
Content-Type: application/json
```

### Body (JSON)
```json
{
  "correo": "testuser@example.com",
  "password": "Password123"
}
```

### Respuestas Esperadas

**? Éxito (200 OK):**
```json
{
  "idUsuario": 1,
  "username": "testuser",
  "correo": "testuser@example.com",
  "passwordHash": null,
  "idRolSistema": 1,
  "creadoEn": "2024-01-15T10:30:00Z"
}
```

**? Error - Credenciales incorrectas (401 Unauthorized):**
```
Correo o contraseña incorrectos.
```

---

## ? 3. GET - Obtener todos los usuarios

### Endpoint
```
GET http://localhost:5120/api/User
```

### Respuestas Esperadas

**? Éxito (200 OK):**
```json
[
  {
    "idUsuario": 1,
    "username": "testuser",
    "correo": "testuser@example.com",
    "idRolSistema": 1,
    "creadoEn": "2024-01-15T10:30:00Z"
  }
]
```

---

## ? 4. GET - Obtener usuario por ID

### Endpoint
```
GET http://localhost:5120/api/User/1
```

### Respuestas Esperadas

**? Éxito (200 OK):**
```json
{
  "idUsuario": 1,
  "username": "testuser",
  "correo": "testuser@example.com",
  "idRolSistema": 1,
  "creadoEn": "2024-01-15T10:30:00Z"
}
```

**? Error - Usuario no encontrado (404 Not Found):**
```
Usuario con ID 999 no encontrado.
```

---

## ? 5. PUT - Actualizar usuario

### Endpoint
```
PUT http://localhost:5120/api/User/1
Content-Type: application/json
```

### Body (JSON)
```json
{
  "idUsuario": 1,
  "username": "testuser_updated",
  "correo": "testuser_updated@example.com",
  "password": "NewPassword456",
  "idRolSistema": 2,
  "idEstadoUsuario": 1
}
```

**Nota:** El campo `password` es opcional. Si no se envía, la contraseña no se actualiza.

### Respuestas Esperadas

**? Éxito (204 No Content):**
```
(Sin contenido - actualización exitosa)
```

**? Error - ID no coincide (400 Bad Request):**
```json
{
  "Message": "El ID de la ruta no coincide con el ID del usuario a actualizar."
}
```

**? Error - Usuario no encontrado (404 Not Found):**
```
Usuario con ID 1 no encontrado para actualizar.
```

**? Error - Username duplicado (409 Conflict):**
```json
{
  "message": "El username ya está en uso por otro usuario."
}
```

---

## ? 6. DELETE - Eliminar usuario

### Endpoint
```
DELETE http://localhost:5120/api/User/1
```

### Respuestas Esperadas

**? Éxito (204 No Content):**
```
(Sin contenido - eliminación exitosa)
```

**? Error - Usuario no encontrado (404 Not Found):**
```
Usuario con ID 1 no encontrado para eliminar.
```

---

## ?? Comandos CURL para Pruebas Rápidas

### 1. Crear Usuario
```bash
curl -X POST http://localhost:5120/api/User \
-H "Content-Type: application/json" \
-d "{\"username\":\"testuser\",\"correo\":\"testuser@example.com\",\"password\":\"Password123\",\"idRolSistema\":1,\"idEstadoUsuario\":1}"
```

### 2. Login
```bash
curl -X POST http://localhost:5120/api/User/SignIn \
-H "Content-Type: application/json" \
-d "{\"correo\":\"testuser@example.com\",\"password\":\"Password123\"}"
```

### 3. Obtener todos los usuarios
```bash
curl -X GET http://localhost:5120/api/User
```

### 4. Obtener usuario por ID
```bash
curl -X GET http://localhost:5120/api/User/1
```

### 5. Actualizar usuario
```bash
curl -X PUT http://localhost:5120/api/User/1 \
-H "Content-Type: application/json" \
-d "{\"idUsuario\":1,\"username\":\"testuser_updated\",\"correo\":\"testuser_updated@example.com\",\"password\":\"NewPassword456\",\"idRolSistema\":2,\"idEstadoUsuario\":1}"
```

### 6. Eliminar usuario
```bash
curl -X DELETE http://localhost:5120/api/User/1
```

---

## ?? Errores Comunes y Soluciones

### Error: "An error occurred while saving the entity changes"
**Causa:** Clave foránea inválida (IdRolSistema o IdEstadoUsuario no existen en la BD)

**Solución:** 
1. Verifica que existan registros en las tablas `roles_sistema` y `estado_usuario_catalogo`
2. Usa IDs válidos en tu request

**SQL para verificar roles:**
```sql
SELECT * FROM roles_sistema;
SELECT * FROM estado_usuario_catalogo;
```

### Error: "The INSERT statement conflicted with the FOREIGN KEY constraint"
**Causa:** El `idRolSistema` o `idEstadoUsuario` no existe en la base de datos

**Solución:**
```sql
-- Insertar rol básico si no existe
INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
VALUES ('USER', 'Usuario', 'Rol de usuario básico', 1);

-- Insertar estado activo si no existe
INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
VALUES ('ACTIVO', 'Usuario activo', 1);
```

### Error: 404 Not Found al hacer POST
**Causa:** La ruta del endpoint no es correcta

**Solución:** Asegúrate de usar exactamente:
- Crear: `POST http://localhost:5120/api/User`
- Login: `POST http://localhost:5120/api/User/SignIn`

---

## ?? Estado de Códigos HTTP

| Código | Significado | Cuándo ocurre |
|--------|-------------|---------------|
| 200 OK | Éxito | GET, SignIn exitoso |
| 201 Created | Recurso creado | POST Usuario exitoso |
| 204 No Content | Éxito sin contenido | PUT, DELETE exitosos |
| 400 Bad Request | Datos inválidos | Validación fallida, IDs no coinciden |
| 401 Unauthorized | No autorizado | Login fallido |
| 404 Not Found | No encontrado | Usuario no existe |
| 409 Conflict | Conflicto | Username o correo duplicados |

---

## ?? Flujo de Prueba Recomendado

1. **Verificar que existan roles y estados en la BD**
2. **Crear un usuario** (POST /api/User)
3. **Iniciar sesión con ese usuario** (POST /api/User/SignIn)
4. **Obtener lista de usuarios** (GET /api/User)
5. **Obtener usuario específico** (GET /api/User/1)
6. **Actualizar el usuario** (PUT /api/User/1)
7. **Eliminar el usuario** (DELETE /api/User/1)

---

## ??? Herramientas Recomendadas

- **Postman** - Cliente HTTP gráfico
- **cURL** - Cliente HTTP por línea de comandos
- **Swagger/OpenAPI** - Accede a `http://localhost:5120/openapi/v1.json`
- **Thunder Client** (VS Code Extension)
- **REST Client** (VS Code Extension)
