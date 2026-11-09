using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/personajes")]
public class PersonajesController(AppDbContext db, ApiService service) : ControllerBase
{
    [HttpGet]
    public async Task<object> Listar()
    {
        var personajes = await db.Personajes.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        return new { data = personajes.Select(service.Map) };
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Uno(int id)
    {
        var personaje = await service.Personaje(id);
        return personaje is null ? NotFound() : Ok(personaje);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(PersonajeRequest request)
    {
        var personaje = new Personaje
        {
            Nombre = request.Nombre.Trim(),
            Especie = request.Especie.Trim(),
            Faccion = request.Faccion.Trim(),
            Afiliacion = request.Afiliacion.Trim(),
            Estado = request.Estado.Trim().ToLowerInvariant(),
            FuerzaSensitivo = request.FuerzaSensitivo,
            Image = request.Image?.Trim() ?? ""
        };
        db.Add(personaje);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Uno), new { id = personaje.Id }, service.Map(personaje));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, PersonajeRequest request)
    {
        var personaje = await db.Personajes.FindAsync(id);
        if (personaje is null) return NotFound();
        personaje.Nombre = request.Nombre.Trim();
        personaje.Especie = request.Especie.Trim();
        personaje.Faccion = request.Faccion.Trim();
        personaje.Afiliacion = request.Afiliacion.Trim();
        personaje.Estado = request.Estado.Trim().ToLowerInvariant();
        personaje.FuerzaSensitivo = request.FuerzaSensitivo;
        personaje.Image = request.Image?.Trim() ?? "";
        await db.SaveChangesAsync();
        return Ok(service.Map(personaje));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var personaje = await db.Personajes.Include(x => x.Cartas).FirstOrDefaultAsync(x => x.Id == id);
        if (personaje is null) return NotFound();
        if (personaje.Cartas.Count > 0) return Conflict(new { mensaje = "El personaje tiene cartas asociadas." });
        db.Remove(personaje);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
