using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/usuarios")]
public class UsuariosController(AppDbContext db, ApiService service, CifradoService cifrado, IPasswordHasher<Usuario> hasher) : ControllerBase
{
    private object ADto(Usuario usuario) => new { usuario.Id, usuario.Nombre, usuario.Email, Telefono = usuario.Telefono is null ? null : cifrado.Descifrar(usuario.Telefono) };

    [HttpGet]
    public async Task<object> Listar() => new { data = (await db.Usuarios.AsNoTracking().ToListAsync()).Select(ADto) };

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Uno(int id) => (await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } usuario ? Ok(ADto(usuario)) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Crear(UsuarioRequest request)
    {
        if (await db.Usuarios.AnyAsync(x => x.Email == request.Email.Trim().ToLower())) return Conflict(new { mensaje = "El correo ya existe." });
        var usuario = await service.CrearUsuario(request);
        return CreatedAtAction(nameof(Uno), new { id = usuario.Id }, ADto(usuario));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, UsuarioRequest request)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();
        var email = request.Email.Trim().ToLower();
        if (await db.Usuarios.AnyAsync(x => x.Email == email && x.Id != id)) return Conflict(new { mensaje = "El correo ya existe." });
        usuario.Nombre = request.Nombre.Trim();
        usuario.Email = email;
        usuario.PasswordHash = hasher.HashPassword(usuario, request.Password);
        usuario.Telefono = string.IsNullOrWhiteSpace(request.Telefono) ? null : cifrado.Cifrar(request.Telefono);
        await db.SaveChangesAsync();
        return Ok(ADto(usuario));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();
        db.Remove(usuario);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
