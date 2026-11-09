using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

public class ApiService(AppDbContext db, CifradoService cifrado, IPasswordHasher<Usuario> hasher)
{
    public async Task<PersonajeDto?> Personaje(int id)
    {
        var personaje = await db.Personajes.FindAsync(id);
        return personaje is null ? null : Map(personaje);
    }

    public PersonajeDto Map(Personaje personaje) => new(personaje.Id, personaje.Nombre, personaje.Especie, personaje.Faccion, personaje.Afiliacion, personaje.Estado, personaje.FuerzaSensitivo, personaje.Image);

    public async Task<List<FaccionDto>> Estadisticas()
    {
        var personajes = await db.Personajes.Include(x => x.Cartas).AsNoTracking().ToListAsync();
        var eventos = await db.Eventos.AsNoTracking().ToListAsync();
        return personajes.GroupBy(x => x.Faccion.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(grupo => new FaccionDto(grupo.Key, grupo.Count(), grupo.Count(x => string.Equals(x.Estado, "vivo", StringComparison.OrdinalIgnoreCase)), grupo.Count(x => string.Equals(x.Estado, "muerto", StringComparison.OrdinalIgnoreCase)), grupo.Sum(x => x.Cartas.Count), grupo.SelectMany(x => x.Cartas).Any() ? grupo.SelectMany(x => x.Cartas).Average(x => x.Poder) : null, eventos.Count(x => string.Equals(x.FaccionGanadora, grupo.Key, StringComparison.OrdinalIgnoreCase)), Math.Round(grupo.Count(x => string.Equals(x.Estado, "muerto", StringComparison.OrdinalIgnoreCase)) * 100.0 / grupo.Count(), 2)))
            .OrderBy(x => x.Faccion).ToList();
    }

    public async Task<Usuario> CrearUsuario(UsuarioRequest request)
    {
        var usuario = new Usuario { Nombre = request.Nombre.Trim(), Email = request.Email.Trim().ToLowerInvariant(), Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : cifrado.Cifrar(request.Telefono) };
        usuario.PasswordHash = hasher.HashPassword(usuario, request.Password);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return usuario;
    }
}
