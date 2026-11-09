# FacciStats API

API REST en ASP.NET Core para personajes, cartas, eventos y estadísticas por facción. Cumple la propuesta mediante los endpoints `/api/v1/facciones` y `/api/v1/estadisticas/generales`; incluye filtros, ranking, comparación y paginación.

## Preparación

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

No hay secretos en `appsettings.json`. Los usuarios guardan contraseñas como hash y el teléfono, igual que la biografía secreta del personaje, se cifra con AES-GCM en MySQL y se descifra únicamente al responder la API. Las migraciones `Inicial` y `AgregarBiografiaCifrada` crean y evolucionan el esquema.
