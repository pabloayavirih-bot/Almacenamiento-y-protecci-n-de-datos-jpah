using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/eventos")]
public class EventosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<object> Listar() => new { data = await db.Eventos.AsNoTracking().OrderByDescending(x => x.Fecha).ToListAsync() };

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Uno(int id) => (await db.Eventos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id)) is { } evento ? Ok(evento) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Crear(EventoRequest request)
    {
        var evento = new Evento { Nombre = request.Nombre.Trim(), Fecha = request.Fecha, FaccionGanadora = request.FaccionGanadora.Trim() };
        db.Add(evento);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Uno), new { id = evento.Id }, evento);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, EventoRequest request)
    {
        var evento = await db.Eventos.FindAsync(id);
        if (evento is null) return NotFound();
        evento.Nombre = request.Nombre.Trim();
        evento.Fecha = request.Fecha;
        evento.FaccionGanadora = request.FaccionGanadora.Trim();
        await db.SaveChangesAsync();
        return Ok(evento);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var evento = await db.Eventos.FindAsync(id);
        if (evento is null) return NotFound();
        db.Remove(evento);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
