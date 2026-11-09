# FacciStats API

API REST en ASP.NET Core para personajes, cartas, eventos y estadísticas por facción. Cumple la propuesta mediante los endpoints `/api/v1/facciones` y `/api/v1/estadisticas/generales`; incluye filtros, ranking, comparación y paginación.

## Estructura

```text
FacciStats/
├── Controllers/   # Endpoints de la API
├── Data/          # AppDbContext y configuración EF Core
├── Dtos/          # Requests y respuestas de la API
├── Migrations/    # Historial de cambios de MySQL
├── Models/        # Personaje, Carta, Evento y Usuario
├── Properties/    # Perfil de ejecución local
├── Services/      # Cifrado AES-GCM y estadísticas
├── FacciStats.http
├── Program.cs
├── README.md
└── USO_DE_IA.md
```

## Preparación

### Requisitos previos

- .NET SDK 10: comprobar con `dotnet --version`.
- MySQL Server 8.0 activo en el puerto `3306` (la aplicación está configurada para MySQL `8.0.36`). Este proyecto no usa MariaDB ni XAMPP.
- Herramienta `dotnet-ef`: instalarla si `dotnet ef` no se reconoce con `dotnet tool install --global dotnet-ef`.
- Una base y un usuario local de MySQL con permisos sobre esa base.

Crear la base y usuario MySQL, luego ejecutar:

```powershell
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=facci_stats_db;User=facci_user;Password=TU_CLAVE"
dotnet user-secrets set "Cifrado:Clave" "PEGA_UNA_CLAVE_BASE64_DE_32_BYTES"
dotnet ef database update
dotnet run
```

La clave AES se genera en PowerShell con:

```powershell
$bytes = New-Object byte[] 32; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes); [Convert]::ToBase64String($bytes)
```

No hay secretos en `appsettings.json`. Los usuarios guardan contraseñas como hash y el teléfono se cifra con AES-GCM en MySQL y se descifra únicamente al responder la API. Las migraciones versionan cada cambio del esquema.

El personaje conserva la estructura trabajada en el catálogo galáctico anterior: nombre, especie, facción, afiliación, estado, sensibilidad a la Fuerza e imagen. Al aplicar la migración más reciente se cargan seis personajes, seis cartas y cuatro eventos de ejemplo.

## Comprobación antes de entregar

```powershell
dotnet build
dotnet ef migrations list
dotnet ef database update
dotnet run
```

En Swagger prueba un personaje y un usuario con teléfono. En MySQL los campos `Telefono` y `PasswordHash` no deben mostrar el texto original; las respuestas GET sí deben devolver el teléfono que la API descifra.
