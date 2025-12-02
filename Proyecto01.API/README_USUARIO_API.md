# ?? Guía Rápida - API de Usuario Funcionando

## ? Tu API está lista y funcionando correctamente

### ?? URL Base
```
http://localhost:5120/api/User
```

---

## ?? Pasos para Probar tu API

### **Paso 1: Inicializar Datos Base en la BD** ?? IMPORTANTE

Antes de crear usuarios, necesitas tener roles y estados en la base de datos.

**Ejecuta este script SQL:**
```sql
-- Ubicación: Proyecto01.API/Scripts/01_InitializeBaseData.sql
```

O ejecuta estos comandos directamente:

```sql
USE Proyecto01;

-- Insertar Roles
INSERT INTO roles_sistema (Codigo, Nombre, Descripcion, EsActivo)
VALUES 
('ADMIN', 'Administrador', 'Rol con todos los permisos del sistema', 1),
('USER', 'Usuario', 'Rol de usuario básico', 1),
('GESTOR', 'Gestor', 'Rol de gestor de solicitudes', 1);

-- Insertar Estados
INSERT INTO estado_usuario_catalogo (Codigo, Descripcion, EsActivo)
VALUES 
('ACTIVO', 'Usuario activo en el sistema', 1),
('INACTIVO', 'Usuario inactivo temporalmente', 1),
('BLOQUEADO', 'Usuario bloqueado por seguridad', 1);

-- Verificar
SELECT * FROM roles_sistema;
SELECT * FROM estado_usuario_catalogo;
```

---

### **Paso 2: Asegúrate de que la API esté corriendo**

1. Abre Visual Studio
2. Presiona `F5` o haz clic en "Iniciar"
3. Verifica que se inicie en `http://localhost:5120`

---

### **Paso 3: Prueba los Endpoints**

#### **Opción A: Usar Postman** (Recomendado)

1. Abre Postman
2. Importa el archivo: `Proyecto01.API/Postman_Collection_Usuario.json`
3. Ejecuta los requests en orden

#### **Opción B: Usar cURL** (Terminal)

**1. Crear un usuario:**
```bash
curl -X POST http://localhost:5120/api/User ^
-H "Content-Type: application/json" ^
-d "{\"username\":\"admin\",\"correo\":\"admin@proyecto01.com\",\"password\":\"Admin123456\",\"idRolSistema\":1,\"idEstadoUsuario\":1}"
```

**2. Iniciar sesión:**
```bash
curl -X POST http://localhost:5120/api/User/SignIn ^
-H "Content-Type: application/json" ^
-d "{\"correo\":\"admin@proyecto01.com\",\"password\":\"Admin123456\"}"
```

**3. Obtener todos los usuarios:**
```bash
curl -X GET http://localhost:5120/api/User
```

**4. Obtener usuario por ID:**
```bash
curl -X GET http://localhost:5120/api/User/1
```

**5. Actualizar usuario:**
```bash
curl -X PUT http://localhost:5120/api/User/1 ^
-H "Content-Type: application/json" ^
-d "{\"idUsuario\":1,\"username\":\"admin_updated\",\"correo\":\"admin_updated@proyecto01.com\",\"idRolSistema\":1,\"idEstadoUsuario\":1}"
```

**6. Eliminar usuario:**
```bash
curl -X DELETE http://localhost:5120/api/User/1
```

#### **Opción C: Usar Thunder Client (VS Code Extension)**

1. Instala Thunder Client en VS Code
2. Importa: `Proyecto01.API/Postman_Collection_Usuario.json`
3. Ejecuta los requests

---

## ?? Endpoints Disponibles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/User` | Crear usuario |
| POST | `/api/User/SignIn` | Iniciar sesión |
| GET | `/api/User` | Obtener todos los usuarios |
| GET | `/api/User/{id}` | Obtener usuario por ID |
| PUT | `/api/User/{id}` | Actualizar usuario |
| DELETE | `/api/User/{id}` | Eliminar usuario |

---

## ?? Ejemplo Completo de Flujo de Prueba

### 1?? Crear Usuario Administrador
```json
POST http://localhost:5120/api/User

{
  "username": "admin",
  "correo": "admin@proyecto01.com",
  "password": "Admin123456",
  "idRolSistema": 1,
  "idEstadoUsuario": 1
}
```

**Respuesta Esperada (201 Created):**
```json
{
  "id": 1
}
```

---

### 2?? Iniciar Sesión
```json
POST http://localhost:5120/api/User/SignIn

{
  "correo": "admin@proyecto01.com",
  "password": "Admin123456"
}
```

**Respuesta Esperada (200 OK):**
```json
{
  "idUsuario": 1,
  "username": "admin",
  "correo": "admin@proyecto01.com",
  "passwordHash": null,
  "idRolSistema": 1,
  "creadoEn": "2024-01-15T10:30:00Z"
}
```

---

### 3?? Obtener Todos los Usuarios
```
GET http://localhost:5120/api/User
```

**Respuesta Esperada (200 OK):**
```json
[
  {
    "idUsuario": 1,
    "username": "admin",
    "correo": "admin@proyecto01.com",
    "idRolSistema": 1,
    "creadoEn": "2024-01-15T10:30:00Z"
  }
]
```

---

## ?? Errores Comunes y Soluciones

### ? Error: "The INSERT statement conflicted with the FOREIGN KEY constraint"

**Problema:** No existen roles o estados en la base de datos.

**Solución:** Ejecuta el script SQL del Paso 1.

---

### ? Error: "El username ya existe" (409 Conflict)

**Problema:** Ya existe un usuario con ese username.

**Solución:** Usa un username diferente o elimina el usuario existente.

---

### ? Error: "Correo o contraseña incorrectos" (401 Unauthorized)

**Problema:** Las credenciales de login son incorrectas.

**Solución:** Verifica que el correo y contraseña sean correctos.

---

### ? Error: 404 Not Found al hacer POST

**Problema:** La URL no es correcta.

**Solución:** Verifica que uses exactamente `http://localhost:5120/api/User`

---

## ?? Validaciones Implementadas

### Crear Usuario (POST)
- ? Username: Requerido, 3-50 caracteres, único
- ? Correo: Requerido, formato email válido, único
- ? Password: Requerido, mínimo 8 caracteres
- ? IdRolSistema: Requerido, mayor a 0, debe existir en BD
- ? IdEstadoUsuario: Valor por defecto 1, debe existir en BD

### Actualizar Usuario (PUT)
- ? Todas las validaciones de creación
- ? Password es opcional (si no se envía, no se actualiza)
- ? No puede usar username o correo de otro usuario

### Login (SignIn)
- ? Correo y Password requeridos
- ? Verifica hash con BCrypt
- ? No retorna el PasswordHash al cliente

---

## ?? IDs de Roles y Estados

Después de ejecutar el script de inicialización:

### Roles
- `1` - ADMIN (Administrador)
- `2` - USER (Usuario)
- `3` - GESTOR (Gestor)

### Estados
- `1` - ACTIVO
- `2` - INACTIVO
- `3` - BLOQUEADO

---

## ?? ¡Tu API está lista!

Todos los endpoints están funcionando correctamente:
- ? Crear usuarios con validaciones
- ? Login con verificación de contraseña
- ? Consultar usuarios
- ? Actualizar usuarios
- ? Eliminar usuarios
- ? Manejo de errores completo

---

## ?? Documentación Adicional

- **Guía de Testing Completa**: `Proyecto01.API/TESTING_GUIDE_USUARIO.md`
- **Colección Postman**: `Proyecto01.API/Postman_Collection_Usuario.json`
- **Script SQL**: `Proyecto01.API/Scripts/01_InitializeBaseData.sql`

---

## ?? ¿Problemas?

Si sigues teniendo problemas:

1. Verifica que la base de datos esté corriendo
2. Verifica la cadena de conexión en `appsettings.json`
3. Ejecuta el script SQL de inicialización
4. Reinicia la API (Ctrl+F5)
5. Revisa los logs en Visual Studio

---

**¡Feliz coding! ??**
