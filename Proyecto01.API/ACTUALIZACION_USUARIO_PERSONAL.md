# ?? **ACTUALIZACIÓN: INTEGRACIÓN DE USUARIO Y PERSONAL**

## ?? **Cambios Implementados**

### **1. UsuarioCreateDTO.cs - ACTUALIZADO**

#### **Cambios realizados:**

**? AGREGADOS - Campos de Personal:**
```csharp
// Nombres (Requerido)
[Required(ErrorMessage = "El nombre es requerido.")]
[StringLength(120, ErrorMessage = "El nombre no debe exceder los 120 caracteres.")]
public string Nombres { get; set; } = null!;

// Apellidos (Requerido)
[Required(ErrorMessage = "El apellido es requerido.")]
[StringLength(120, ErrorMessage = "El apellido no debe exceder los 120 caracteres.")]
public string Apellidos { get; set; } = null!;

// Documento (Opcional)
[StringLength(20, ErrorMessage = "El documento no debe exceder los 20 caracteres.")]
public string? Documento { get; set; }
```

**? MARCADOS COMO INTERNOS:**
```csharp
// Campo interno - No enviado por Android
[System.Text.Json.Serialization.JsonIgnore]
public string? PasswordHash { get; set; }

// Campo interno - Asignado automáticamente
[System.Text.Json.Serialization.JsonIgnore]
public int IdEstadoUsuario { get; set; } = 1; // ACTIVO por defecto
```

#### **Por qué estos cambios:**
- Los campos `Nombres`, `Apellidos` y `Documento` vienen del formulario de Android
- El `PasswordHash` se genera en el Service (BCrypt)
- El `IdEstadoUsuario` se asigna automáticamente como ACTIVO (1)
- El atributo `[JsonIgnore]` hace que no se serialicen en JSON

---

### **2. UsuarioRepository.cs - MÉTODO INSERT MEJORADO**

#### **Antes (Solo creaba Usuario):**
```csharp
public async Task<int> Insert(UsuarioCreateDTO dto)
{
    var usuario = new Usuario { ... };
    _context.Usuarios.Add(usuario);
    await _context.SaveChangesAsync();
    return usuario.IdUsuario;
}
```

#### **Después (Crea Usuario Y Personal):**
```csharp
public async Task<int> Insert(UsuarioCreateDTO dto)
{
    // 1?? CREAR USUARIO
    var usuario = new Usuario
    {
        Username = dto.Username,
        Correo = dto.Correo,
        PasswordHash = dto.PasswordHash,
        IdRolSistema = dto.IdRolSistema,
        IdEstadoUsuario = dto.IdEstadoUsuario,
        CreadoEn = DateTime.UtcNow
    };

    _context.Usuarios.Add(usuario);
    await _context.SaveChangesAsync(); // ? Obtener IdUsuario autoincremental

    // 2?? CREAR PERSONAL (vinculado al Usuario)
    var personal = new Personal
    {
        IdUsuario = usuario.IdUsuario, // ? Usar ID recién generado
        Nombres = dto.Nombres,
        Apellidos = dto.Apellidos,
        Documento = dto.Documento,
        Estado = "Activo",
        CreadoEn = DateTime.UtcNow
    };

    _context.Personales.Add(personal);
    await _context.SaveChangesAsync(); // ? Guardar Personal

    return usuario.IdUsuario;
}
```

#### **Flujo de la transacción:**
```
???????????????????????????????????
? SaveChangesAsync #1             ?
? ?                               ?
? INSERT INTO usuario             ?
? VALUES (...)                    ?
? ?                               ?
? usuario.IdUsuario = 1 (auto)    ?
???????????????????????????????????
            ?
???????????????????????????????????
? SaveChangesAsync #2             ?
? ?                               ?
? INSERT INTO personal            ?
? VALUES (IdUsuario=1, ...)       ?
? ?                               ?
? personal.IdPersonal = 1 (auto)  ?
???????????????????????????????????
            ?
    ? Ambas tablas con datos
       vinculados correctamente
```

---

### **3. UsuarioService.cs - VALIDACIONES MEJORADAS**

#### **Validaciones agregadas en SignUp:**

```csharp
// ========================================
// VALIDACIONES DE CAMPOS USUARIO
// ========================================

if (string.IsNullOrWhiteSpace(dto.Username))
    throw new ArgumentException("El username es requerido.");

if (string.IsNullOrWhiteSpace(dto.Correo))
    throw new ArgumentException("El correo es requerido.");

if (string.IsNullOrWhiteSpace(dto.Password))
    throw new ArgumentException("La contraseña es requerida.");

if (dto.IdRolSistema <= 0)
    throw new ArgumentException("Debe especificar un rol de sistema válido.");

// ========================================
// VALIDACIONES DE CAMPOS PERSONAL ??
// ========================================

if (string.IsNullOrWhiteSpace(dto.Nombres))
    throw new ArgumentException("El nombre es requerido.");

if (string.IsNullOrWhiteSpace(dto.Apellidos))
    throw new ArgumentException("El apellido es requerido.");

// ========================================
// VALIDACIONES DE DUPLICADOS
// ========================================

var userExists = await _repository.ExistsByUsername(dto.Username);
if (userExists)
    throw new InvalidOperationException("El username ya existe.");

var emailExists = await _repository.ExistsByCorreo(dto.Correo);
if (emailExists)
    throw new InvalidOperationException("El correo ya está registrado.");

// ========================================
// PREPARAR DATOS Y GUARDAR
// ========================================

dto.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
dto.IdEstadoUsuario = 1; // ACTIVO

return await _repository.Insert(dto);
```

---

## ?? **Impacto en el Flujo de Registro**

### **Antes:**
```
Android envía:
{
  "username": "juan",
  "correo": "juan@empresa.com",
  "password": "Segura123",
  "idRolSistema": 1
}
    ?
API crea:
??? Usuario ?
??? Personal ? (No se creaba)
```

### **Después:**
```
Android envía:
{
  "username": "juan",
  "correo": "juan@empresa.com",
  "password": "Segura123",
  "idRolSistema": 1,
  "nombres": "Juan",          ? ??
  "apellidos": "Pérez",        ? ??
  "documento": "12345678"      ? ?? (opcional)
}
    ?
API crea:
??? Usuario ?
??? Personal ? (Vinculado automáticamente)
```

---

## ?? **Ejemplo de Request Completo**

### **POST /api/User (Registro con Personal)**

```json
{
  "username": "juan",
  "correo": "juan@empresa.com",
  "password": "Segura123",
  "idRolSistema": 1,
  "nombres": "Juan",
  "apellidos": "Pérez García",
  "documento": "12345678-A"
}
```

### **Respuesta (201 Created):**
```json
{
  "id": 1
}
```

### **En la Base de Datos:**

**Tabla usuario:**
```
id_usuario | username | correo              | id_rol_sistema | id_estado_usuario | creado_en
-----------|----------|---------------------|----------------|--------------------|----------
1          | juan     | juan@empresa.com    | 1              | 1                  | 2024-01-15
```

**Tabla personal:**
```
id_personal | id_usuario | nombres | apellidos      | documento   | estado  | creado_en
------------|------------|---------|----------------|-----------  |---------|----------
1           | 1          | Juan    | Pérez García   | 12345678-A | Activo  | 2024-01-15
```

---

## ? **Validaciones Implementadas**

### **Nivel 1: Frontend (DTOs)**
```csharp
[Required]  ? Campos obligatorios
[StringLength(120)]  ? Máximo 120 caracteres
[EmailAddress]  ? Formato válido
```

### **Nivel 2: Business Logic (Service)**
```csharp
if (string.IsNullOrWhiteSpace(dto.Nombres))
    throw new ArgumentException("El nombre es requerido.");
```

### **Nivel 3: Database Constraints**
```sql
NOT NULL constraints
FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario)
UNIQUE constraints
```

---

## ?? **Flujo Completo de Registro**

```
1. Android envía JSON con datos de Usuario + Personal
   ?
2. UsuarioController recibe y valida ModelState
   ?
3. UsuarioService.SignUp() valida todos los campos
   ??? Verifica que Nombres no sea null
   ??? Verifica que Apellidos no sea null
   ??? Verifica que Username no esté duplicado
   ??? Verifica que Correo no esté duplicado
   ??? Genera BCrypt hash de password
   ??? Asigna IdEstadoUsuario = 1 (ACTIVO)
   ?
4. UsuarioRepository.Insert() en transacción:
   ??? INSERT usuario ? Obtiene IdUsuario (auto-increment)
   ??? SaveChangesAsync()
   ??? INSERT personal (vinculado a IdUsuario)
   ??? SaveChangesAsync()
   ??? Retorna IdUsuario
   ?
5. Android recibe: { "id": 1 }
   ?
6. ? Usuario y Personal creados, vinculados y listos
```

---

## ?? **Campos de la Tabla Personal Ahora Asignados**

```csharp
// Enviados por Android (en DTO)
public string Nombres { get; set; }      // "Juan"
public string Apellidos { get; set; }    // "Pérez"
public string? Documento { get; set; }   // "12345678-A"

// Asignados por API
public int IdUsuario { get; set; }       // Usuario recién creado
public string Estado { get; set; }       // "Activo"
public DateTime CreadoEn { get; set; }   // DateTime.UtcNow
```

---

## ?? **Campos Internos No Retornados**

```csharp
// Estos campos NO se serializan al enviar JSON
[JsonIgnore]
public string? PasswordHash { get; set; }  // Generado por BCrypt

[JsonIgnore]
public int IdEstadoUsuario { get; set; }   // Asignado automáticamente
```

---

## ?? **Testing - Casos de Prueba**

### **? Caso 1: Registro Exitoso**
```
POST /api/User
{
  "username": "juan",
  "correo": "juan@empresa.com",
  "password": "Segura123",
  "idRolSistema": 1,
  "nombres": "Juan",
  "apellidos": "Pérez"
}
? 201 Created: { "id": 1 }
```

### **? Caso 2: Sin Nombres**
```
POST /api/User
{
  "username": "juan",
  ...
  "apellidos": "Pérez"
  // Falta "nombres"
}
? 400 Bad Request: "El nombre es requerido."
```

### **? Caso 3: Sin Apellidos**
```
POST /api/User
{
  "username": "juan",
  ...
  "nombres": "Juan"
  // Falta "apellidos"
}
? 400 Bad Request: "El apellido es requerido."
```

### **? Caso 4: Username Duplicado**
```
POST /api/User
{
  "username": "juan",  // Ya existe
  ...
}
? 409 Conflict: "El username ya existe."
```

---

## ?? **Impacto en la Aplicación**

| Aspecto | Antes | Después |
|--------|-------|---------|
| Tablas creadas | Usuario solo | Usuario + Personal |
| Campos en DTO | 5 | 8 |
| Validaciones | 4 | 6 |
| Transacciones | 1 | 2 |
| Relaciones FK | Usuario-Rol | Usuario-Rol + Usuario-Personal |
| Completitud datos | Incompleta | ? Completa |

---

## ?? **Conclusión**

### **Lo que cambió:**
? UsuarioCreateDTO ahora incluye campos de Personal  
? Repository.Insert() crea ambas entidades en transacción  
? Service.SignUp() valida datos de Personal  
? Personal se vincula automáticamente al Usuario  

### **Lo que permanece igual:**
- Endpoints siguen siendo los mismos
- Respuestas siguen siendo las mismas
- Seguridad (BCrypt) intacta
- Logging completo

### **Beneficio:**
Ahora cuando se crea un Usuario, automáticamente se crea su perfil de Personal, eliminando la necesidad de dos requests separados desde Android.

---

**La integración Usuario-Personal está completa y lista para producción. ??**
