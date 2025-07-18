📘 NotiCheck API Documentation

NotiCheck es una solución móvil y backend desarrollada en Android (Kotlin) y .NET Core 8, que permite registrar, analizar y gestionar notificaciones de pagos como Yape, Plin y otras apps seleccionadas por el usuario.

🔐 Seguridad
- Autenticación JWT para validar al usuario.
- APIKey obligatoria para proteger endpoints públicos como /login y /register.

🧪 Variables sugeridas para Postman
- baseUrl = https://verificador.micreativa.com
- apiKey  = Valor de la tabla Parametros donde Clave='ApiKey'
- token   = JWT del usuario (obtenido al hacer login)

===============================
AUTENTICACIÓN (/api/auth)
===============================

🔸 POST /api/auth/register
Header:
  Authorization: Bearer {{apiKey}}

Body:
{
  "username": "demo22",
  "password": "demo12322"
}

Respuesta:
{ "message": "Usuario registrado con trial" }

---

🔸 POST /api/auth/login
Header:
  Authorization: Bearer {{apiKey}}

Body:
{
  "username": "demo22",
  "password": "demo12322"
}

Respuesta:
{ "token": "eyJhbGciOi..." }

---

🔸 GET /api/auth/perfil
Header:
  Authorization: Bearer {{token}}

Respuesta:
{
  "mensaje": "Bienvenido demo22 (ID: 3), estás autenticado"
}

---

🔸 PUT /api/auth/cambiar-password
Header:
  Authorization: Bearer {{token}}

Body:
{
  "passwordActual": "demo12322",
  "passwordNuevo": "nuevo123"
}

---

🔸 POST /api/auth/revocar-tokens
Header:
  Authorization: Bearer {{token}}

Revoca todos los tokens anteriores emitidos al usuario.

===============================
NOTIFICACIONES (/api/notificaciones)
===============================

🔸 GET /api/notificaciones
Header:
  Authorization: Bearer {{token}}

Respuesta:
[
  {
    "id": 41,
    "mensajeCompleto": "Yape! Christian te envió un pago por S/ 1.00",
    "persona": "Christian",
    "monto": 1.00,
    "fechaNotificacion": "2025-07-16T18:32:31",
    "createdAt": "2025-07-16T23:32:33",
    "vista": false
  }
]

---

🔸 DELETE /api/notificaciones/{id}
Header:
  Authorization: Bearer {{token}}

---

🔸 PUT /api/notificaciones/{id}/marcar-visto
Header:
  Authorization: Bearer {{token}}

===============================
APLICACIONES DEL USUARIO (/api/usuario-aplicaciones)
===============================

🔸 GET /api/usuario-aplicaciones
Header:
  Authorization: Bearer {{token}}

---

🔸 POST /api/usuario-aplicaciones
Header:
  Authorization: Bearer {{token}}

Body:
{
  "aplicacionId": 1
}

---

🔸 DELETE /api/usuario-aplicaciones/{id}
Header:
  Authorization: Bearer {{token}}

===============================
USUARIO (/api/usuarios)
===============================

🔸 PUT /api/usuarios/nombre
Header:
  Authorization: Bearer {{token}}

Body:
{
  "nombre": "Christian Luis"
}

===============================
ENLACE CORTO (/api/enlace/generar)
===============================

🔸 GET /api/enlace/generar
Header:
  Authorization: Bearer {{token}}

Devuelve un enlace del tipo:
https://noticheckapp.com?8iie8={usuarioCifrado}&t94fh={tokenCifrado}

===============================
ESTRUCTURA DE UNA NOTIFICACIÓN
===============================

- id: int
- mensajeCompleto: string
- persona: string
- monto: decimal?
- fechaNotificacion: datetime
- createdAt: datetime
- vista: bool

