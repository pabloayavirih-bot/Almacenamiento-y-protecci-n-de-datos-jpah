using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FacciStats;

[ApiController]
[Route("api/v1/facciones")]
public class FaccionesController(AppDbContext db, ApiService service) : ControllerBase
{
    [HttpGet]
    public async Task<object> Lista() => new { data = (await service.Estadisticas()).Select(x => new { x.Faccion, x.Personajes }) };

    [HttpGet("estadisticas")]
    public async Task<object> Estadisticas(string? ordenarPor = null, string direccion = "asc")
    {
        var estadisticas = await service.Estadisticas();
        if (ordenarPor == "poderPromedio") estadisticas = direccion == "desc" ? estadisticas.OrderByDescending(x => x.PoderPromedio).ToList() : estadisticas.OrderBy(x => x.PoderPromedio).ToList();
        return new { data = estadisticas };
    }

    [HttpGet("{nombre}/estadisticas")]
    public async Task<IActionResult> Una(string nombre)
    {
        var faccion = (await service.Estadisticas()).FirstOrDefault(x => string.Equals(x.Faccion, nombre, StringComparison.OrdinalIgnoreCase));
        return faccion is null ? NotFound() : Ok(new { data = faccion });
    }

    [HttpGet("{nombre}/personajes")]
    public async Task<IActionResult> Personajes(string nombre, string estado = "todos", int pagina = 1, int tamano = 10)
    {
        if (pagina < 1 || tamano is < 1 or > 50) return BadRequest(new { mensaje = "Paginación inválida." });
        var consulta = db.Personajes.AsNoTracking().Where(x => x.Faccion == nombre);
        if (estado == "vivos") consulta = consulta.Where(x => !x.EstaMuerto);
        else if (estado == "muertos") consulta = consulta.Where(x => x.EstaMuerto);
        else if (estado != "todos") return BadRequest(new { mensaje = "Estado inválido." });
        var total = await consulta.CountAsync();
        return Ok(new { data = await consulta.Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(), meta = new { pagina, tamano, total } });
    }

    [HttpGet("{nombre}/cartas")]
    public async Task<object> Cartas(string nombre, int? poderMinimo = null, string orden = "asc")
    {
        var consulta = db.Cartas.Include(x => x.Personaje).Where(x => x.Personaje!.Faccion == nombre);
        if (poderMinimo.HasValue) consulta = consulta.Where(x => x.Poder >= poderMinimo);
        var cartas = orden == "desc" ? consulta.OrderByDescending(x => x.Poder) : consulta.OrderBy(x => x.Poder);
        return new { data = await cartas.Select(x => new { x.Id, x.Nombre, x.Poder, personaje = x.Personaje!.Nombre }).ToListAsync() };
    }

    [HttpGet("{nombre}/eventos-ganados")]
    public async Task<object> Eventos(string nombre, DateTime? desde = null, DateTime? hasta = null)
    {
        var consulta = db.Eventos.Where(x => x.FaccionGanadora == nombre);
        if (desde.HasValue) consulta = consulta.Where(x => x.Fecha >= desde);
        if (hasta.HasValue) consulta = consulta.Where(x => x.Fecha <= hasta);
        return new { data = await consulta.OrderByDescending(x => x.Fecha).ToListAsync() };
    }

    [HttpGet("ranking")]
    public async Task<IActionResult> Ranking(string criterio = "personajes", int top = 10)
    {
        if (top is < 1 or > 20) return BadRequest();
        var estadisticas = await service.Estadisticas();
        var ordenado = criterio switch
        {
            "poderPromedio" => estadisticas.OrderByDescending(x => x.PoderPromedio),
            "eventosGanados" => estadisticas.OrderByDescending(x => x.EventosGanados),
            "personajes" => estadisticas.OrderByDescending(x => x.Personajes),
            _ => null
        };
        if (ordenado is null) return BadRequest(new { mensaje = "Criterio inválido." });
        return Ok(new { data = ordenado.Take(top).Select((x, indice) => new { posicion = indice + 1, x.Faccion, valor = criterio == "personajes" ? x.Personajes : criterio == "eventosGanados" ? x.EventosGanados : x.PoderPromedio }) });
    }

    [HttpGet("comparar")]
    public async Task<IActionResult> Comparar(string faccionA, string faccionB)
    {
        if (string.Equals(faccionA, faccionB, StringComparison.OrdinalIgnoreCase)) return BadRequest();
        var estadisticas = await service.Estadisticas();
        var primera = estadisticas.FirstOrDefault(x => x.Faccion == faccionA);
        var segunda = estadisticas.FirstOrDefault(x => x.Faccion == faccionB);
        return primera is null || segunda is null ? NotFound() : Ok(new { faccionA = primera, faccionB = segunda, diferencias = new { personajes = primera.Personajes - segunda.Personajes, eventosGanados = primera.EventosGanados - segunda.EventosGanados } });
    }
}
