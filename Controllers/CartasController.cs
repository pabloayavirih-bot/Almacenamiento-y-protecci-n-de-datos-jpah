using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/cartas")]
public class CartasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<object> Listar() => new { data = await db.Cartas.Include(x => x.Personaje).AsNoTracking().Select(x => new { x.Id, x.Nombre, x.Poder, x.PersonajeId, personaje = x.Personaje!.Nombre }).ToListAsync() };

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Uno(int id)
    {
        var carta = await db.Cartas.Include(x => x.Personaje).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return carta is null ? NotFound() : Ok(new { carta.Id, carta.Nombre, carta.Poder, carta.PersonajeId, personaje = carta.Personaje!.Nombre });
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CartaRequest request)
    {
        if (!await db.Personajes.AnyAsync(x => x.Id == request.PersonajeId)) return BadRequest(new { mensaje = "Personaje inexistente." });
        var carta = new Carta { PersonajeId = request.PersonajeId, Nombre = request.Nombre.Trim(), Poder = request.Poder };
        db.Add(carta);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Uno), new { id = carta.Id }, carta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, CartaRequest request)
    {
        var carta = await db.Cartas.FindAsync(id);
        if (carta is null) return NotFound();
        if (!await db.Personajes.AnyAsync(x => x.Id == request.PersonajeId)) return BadRequest(new { mensaje = "Personaje inexistente." });
        carta.PersonajeId = request.PersonajeId;
        carta.Nombre = request.Nombre.Trim();
        carta.Poder = request.Poder;
        await db.SaveChangesAsync();
        return Ok(carta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var carta = await db.Cartas.FindAsync(id);
        if (carta is null) return NotFound();
        db.Remove(carta);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
