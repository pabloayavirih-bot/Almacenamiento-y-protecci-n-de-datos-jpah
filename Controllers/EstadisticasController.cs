using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/estadisticas")]
public class EstadisticasController(AppDbContext db) : ControllerBase
{
    [HttpGet("generales")]
    public async Task<object> Generales()
    {
        return new
        {
            data = new
            {
                facciones = await db.Personajes.Select(x => x.Faccion).Distinct().CountAsync(),
                personajes = await db.Personajes.CountAsync(),
                vivos = await db.Personajes.CountAsync(x => x.Estado == "vivo"),
                muertos = await db.Personajes.CountAsync(x => x.Estado == "muerto"),
                cartas = await db.Cartas.CountAsync(),
                eventos = await db.Eventos.CountAsync()
            }
        };
    }
}
