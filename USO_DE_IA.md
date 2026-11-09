# Declaración de uso de IA

Usé Codex como apoyo puntual para revisar la organización de carpetas, mejorar el formato del código y preparar una lista de comprobaciones para Swagger y MySQL. Acepté la propuesta de separar controladores por responsabilidad porque hace más fácil explicar cada recurso durante la defensa.

También solicité explicaciones sobre la configuración de Secret Manager, las migraciones de Entity Framework Core y el cifrado AES-GCM. Revisé cada sugerencia antes de aplicarla; adapté los ejemplos a MySQL Server 8 y descarté alternativas que usaban MariaDB, XAMPP o servicios ajenos a lo avanzado en clase. Corregí los problemas encontrados durante las pruebas, como la configuración de la conexión local, el orden correcto de los parámetros al descifrar, el mapeo de biografías descifradas y el orden estable de la paginación.

La implementación fue verificada ejecutando las migraciones, revisando los datos guardados en MySQL y probando los endpoints. No hay contraseñas, cadenas de conexión ni claves de cifrado dentro del repositorio. Entiendo el flujo de los controladores, las consultas LINQ de estadísticas, el hash de contraseñas con `PasswordHasher` y el cifrado de campos sensibles con AES-GCM.
