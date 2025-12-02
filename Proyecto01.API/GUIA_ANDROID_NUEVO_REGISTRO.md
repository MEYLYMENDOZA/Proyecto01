# ?? **GUÍA PARA ANDROID - NUEVO FORMATO DE REGISTRO**

## ?? **Cambios en el Endpoint de Registro**

### **Endpoint:**
```
POST http://localhost:5120/api/User
```

---

## ?? **Formato de Request - ACTUALIZADO**

### **Antes (Formato Antiguo - YA NO FUNCIONA):**
```json
{
  "username": "juan",
  "correo": "juan@empresa.com",
  "password": "Segura123",
  "idRolSistema": 1
}
```

### **Ahora (Formato Nuevo - REQUERIDO):**
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

---

## ?? **Campos Nuevos Requeridos**

| Campo | Tipo | Requerido | Longitud | Ejemplo |
|-------|------|-----------|----------|---------|
| `nombres` | string | ? SÍ | 1-120 | "Juan" |
| `apellidos` | string | ? SÍ | 1-120 | "Pérez García" |
| `documento` | string | ? NO | 1-20 | "12345678-A" |

---

## ? **Validaciones en Android**

### **Antes de enviar a la API:**

```kotlin
// Validación de Nombres
if (nombres.isBlank()) {
    Toast.makeText(context, "El nombre es requerido", Toast.LENGTH_SHORT).show()
    return
}

// Validación de Apellidos
if (apellidos.isBlank()) {
    Toast.makeText(context, "El apellido es requerido", Toast.LENGTH_SHORT).show()
    return
}

// Validación de Nombres (Longitud)
if (nombres.length > 120) {
    Toast.makeText(context, "El nombre no debe exceder 120 caracteres", Toast.LENGTH_SHORT).show()
    return
}

// Validación de Apellidos (Longitud)
if (apellidos.length > 120) {
    Toast.makeText(context, "El apellido no debe exceder 120 caracteres", Toast.LENGTH_SHORT).show()
    return
}

// Documento es opcional, pero si se envía, máximo 20 caracteres
if (documento.isNotEmpty() && documento.length > 20) {
    Toast.makeText(context, "El documento no debe exceder 20 caracteres", Toast.LENGTH_SHORT).show()
    return
}
```

---

## ?? **Ejemplo de Implementación en Android (Kotlin)**

### **Data Class (Modelo):**
```kotlin
data class RegistroRequest(
    val username: String,
    val correo: String,
    val password: String,
    val idRolSistema: Int,
    val nombres: String,        // ??
    val apellidos: String,      // ??
    val documento: String? = null  // ?? (opcional)
)
```

### **Formulario (Activity/Fragment):**
```kotlin
class RegistroActivity : AppCompatActivity() {
    
    private lateinit var etUsername: EditText
    private lateinit var etCorreo: EditText
    private lateinit var etPassword: EditText
    private lateinit var etNombres: EditText      // ??
    private lateinit var etApellidos: EditText    // ??
    private lateinit var etDocumento: EditText    // ??
    private lateinit var spinnerRol: Spinner
    private lateinit var btnRegistrar: Button

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_registro)
        
        etUsername = findViewById(R.id.et_username)
        etCorreo = findViewById(R.id.et_correo)
        etPassword = findViewById(R.id.et_password)
        etNombres = findViewById(R.id.et_nombres)        // ??
        etApellidos = findViewById(R.id.et_apellidos)    // ??
        etDocumento = findViewById(R.id.et_documento)    // ??
        spinnerRol = findViewById(R.id.spinner_rol)
        btnRegistrar = findViewById(R.id.btn_registrar)
        
        btnRegistrar.setOnClickListener { registrar() }
    }
    
    private fun registrar() {
        val username = etUsername.text.toString().trim()
        val correo = etCorreo.text.toString().trim()
        val password = etPassword.text.toString()
        val nombres = etNombres.text.toString().trim()      // ??
        val apellidos = etApellidos.text.toString().trim()  // ??
        val documento = etDocumento.text.toString().trim()  // ??
        val idRolSistema = 2 // O del spinner
        
        // Validaciones básicas
        if (username.isBlank() || correo.isBlank() || password.isBlank() || 
            nombres.isBlank() || apellidos.isBlank()) {    // ??
            Toast.makeText(this, "Todos los campos requeridos deben completarse", Toast.LENGTH_SHORT).show()
            return
        }
        
        // Crear request
        val request = RegistroRequest(
            username = username,
            correo = correo,
            password = password,
            idRolSistema = idRolSistema,
            nombres = nombres,       // ??
            apellidos = apellidos,   // ??
            documento = documento.ifEmpty { null }  // ??
        )
        
        // Enviar a la API
        llamarAPI(request)
    }
    
    private fun llamarAPI(request: RegistroRequest) {
        val retrofitService = RetrofitClient.getService()
        
        lifecycleScope.launch {
            try {
                val response = retrofitService.registrar(request)
                if (response.isSuccessful) {
                    val usuarioId = response.body()?.id
                    Toast.makeText(this@RegistroActivity, 
                        "Registro exitoso. ID: $usuarioId", 
                        Toast.LENGTH_SHORT).show()
                    
                    // Navegar a login
                    startActivity(Intent(this@RegistroActivity, LoginActivity::class.java))
                    finish()
                } else {
                    // Error en la respuesta
                    val errorBody = response.errorBody()?.string()
                    Toast.makeText(this@RegistroActivity, 
                        "Error: $errorBody", 
                        Toast.LENGTH_SHORT).show()
                }
            } catch (e: Exception) {
                Toast.makeText(this@RegistroActivity, 
                    "Error de conexión: ${e.message}", 
                    Toast.LENGTH_SHORT).show()
            }
        }
    }
}
```

### **Retrofit Interface:**
```kotlin
interface UsuarioService {
    @POST("User")
    suspend fun registrar(@Body request: RegistroRequest): Response<RegistroResponse>
}

data class RegistroResponse(
    val id: Int
)
```

---

## ?? **Layout XML (activity_registro.xml) - ACTUALIZADO**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:padding="16dp">

    <TextView
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="Registro de Usuario"
        android:textSize="24sp"
        android:textStyle="bold"
        android:layout_marginBottom="16dp" />

    <!-- Username -->
    <EditText
        android:id="@+id/et_username"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Nombre de usuario"
        android:inputType="text"
        android:layout_marginBottom="12dp" />

    <!-- Correo -->
    <EditText
        android:id="@+id/et_correo"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Correo electrónico"
        android:inputType="emailAddress"
        android:layout_marginBottom="12dp" />

    <!-- Password -->
    <EditText
        android:id="@+id/et_password"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Contraseña (mínimo 8 caracteres)"
        android:inputType="textPassword"
        android:layout_marginBottom="12dp" />

    <!-- ?? NUEVOS CAMPOS -->

    <!-- Nombres -->
    <EditText
        android:id="@+id/et_nombres"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Nombres (ej: Juan)"
        android:inputType="text"
        android:layout_marginBottom="12dp" />

    <!-- Apellidos -->
    <EditText
        android:id="@+id/et_apellidos"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Apellidos (ej: Pérez García)"
        android:inputType="text"
        android:layout_marginBottom="12dp" />

    <!-- Documento (Opcional) -->
    <EditText
        android:id="@+id/et_documento"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:hint="Documento (opcional)"
        android:inputType="text"
        android:layout_marginBottom="12dp" />

    <!-- Rol -->
    <Spinner
        android:id="@+id/spinner_rol"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:layout_marginBottom="12dp" />

    <!-- Botón Registrar -->
    <Button
        android:id="@+id/btn_registrar"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="REGISTRAR"
        android:layout_marginBottom="12dp" />

    <!-- Botón Login -->
    <Button
        android:id="@+id/btn_ir_login"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:text="¿Ya tienes cuenta? Inicia sesión"
        android:backgroundTint="@android:color/darker_gray" />

</LinearLayout>
```

---

## ?? **Comparación de Requests**

### **Antes (Ya NO funciona):**
```
curl -X POST http://localhost:5120/api/User \
-H "Content-Type: application/json" \
-d '{"username":"juan","correo":"juan@empresa.com","password":"Segura123","idRolSistema":1}'

? Error 400: "El nombre es requerido."
```

### **Después (Nuevo formato):**
```
curl -X POST http://localhost:5120/api/User \
-H "Content-Type: application/json" \
-d '{
  "username":"juan",
  "correo":"juan@empresa.com",
  "password":"Segura123",
  "idRolSistema":1,
  "nombres":"Juan",
  "apellidos":"Pérez",
  "documento":"12345678"
}'

? Respuesta 201: {"id": 1}
```

---

## ?? **Respuestas de Error Esperadas**

### **Sin Nombres:**
```json
HTTP 400 Bad Request
{
  "message": "El nombre es requerido."
}
```

### **Sin Apellidos:**
```json
HTTP 400 Bad Request
{
  "message": "El apellido es requerido."
}
```

### **Nombres muy largo:**
```json
HTTP 400 Bad Request
{
  "message": "El nombre no debe exceder los 120 caracteres."
}
```

### **Documento muy largo:**
```json
HTTP 400 Bad Request
{
  "message": "El documento no debe exceder los 20 caracteres."
}
```

### **Todo correcto:**
```json
HTTP 201 Created
{
  "id": 1
}
```

---

## ?? **Checklist para Actualizar Android**

- [ ] Agregar campos `nombres`, `apellidos`, `documento` al layout
- [ ] Actualizar data class RegistroRequest
- [ ] Agregar validaciones para campos nuevos
- [ ] Actualizar retrofit call
- [ ] Probar con Postman primero
- [ ] Probar en emulador/dispositivo
- [ ] Verificar que Usuario y Personal se crean en BD
- [ ] Revisar logs en Visual Studio

---

## ?? **Nota Importante**

**?? CAMBIO BREAKING:**
- El API ahora REQUIERE `nombres` y `apellidos`
- Apps antiguas que no envíen estos campos recibirán error 400
- Es NECESARIO actualizar el cliente Android

---

## ? **Verificación en BD**

Después de registrarse en Android, verifica en SQL Server:

```sql
-- Ver usuario creado:
SELECT * FROM usuario WHERE username = 'juan';

-- Ver personal creado automáticamente:
SELECT * FROM personal WHERE id_usuario = (SELECT id_usuario FROM usuario WHERE username = 'juan');
```

---

**¡Lista la integración Usuario-Personal! ??**

Actualiza tu app Android y sigue esta guía.
