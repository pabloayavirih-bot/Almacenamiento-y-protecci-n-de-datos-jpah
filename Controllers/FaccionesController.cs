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
    public async Task<IActionResult> Estadisticas(string? ordenarPor = null, string direccion = "asc")
    {
        if (ordenarPor is not null && !string.Equals(ordenarPor, "poderPromedio", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "El parámetro ordenarPor solo acepta poderPromedio." });
        if (!string.Equals(direccion, "asc", StringComparison.OrdinalIgnoreCase) && !string.Equals(direccion, "desc", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "La dirección solo acepta asc o desc." });

        var estadisticas = await service.Estadisticas();
        if (string.Equals(ordenarPor, "poderPromedio", StringComparison.OrdinalIgnoreCase))
            estadisticas = string.Equals(direccion, "desc", StringComparison.OrdinalIgnoreCase)
                ? estadisticas.OrderByDescending(x => x.PoderPromedio).ToList()
                : estadisticas.OrderBy(x => x.PoderPromedio).ToList();
        return Ok(new { data = estadisticas });
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
        var faccionNormalizada = nombre.Trim().ToUpper();
        var consulta = db.Personajes.AsNoTracking().Where(x => x.Faccion.Trim().ToUpper() == faccionNormalizada);
        if (string.Equals(estado, "vivos", StringComparison.OrdinalIgnoreCase) || string.Equals(estado, "vivo", StringComparison.OrdinalIgnoreCase)) consulta = consulta.Where(x => x.Estado == "vivo");
        else if (string.Equals(estado, "muertos", StringComparison.OrdinalIgnoreCase) || string.Equals(estado, "muerto", StringComparison.OrdinalIgnoreCase)) consulta = consulta.Where(x => x.Estado == "muerto");
        else if (!string.Equals(estado, "todos", StringComparison.OrdinalIgnoreCase)) return BadRequest(new { mensaje = "Estado inválido. Usa todos, vivos o muertos." });
        var total = await consulta.CountAsync();
        var personajes = await consulta.OrderBy(x => x.Id).Skip((pagina - 1) * tamano).Take(tamano).ToListAsync();
        return Ok(new { data = personajes.Select(service.Map), meta = new { pagina, tamano, total } });
    }

    [HttpGet("{nombre}/cartas")]
    public async Task<IActionResult> Cartas(string nombre, int? poderMinimo = null, string orden = "asc")
    {
        if (!string.Equals(orden, "asc", StringComparison.OrdinalIgnoreCase) && !string.Equals(orden, "desc", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "El orden solo acepta asc o desc." });

        var faccionNormalizada = nombre.Trim().ToUpper();
        var consulta = db.Cartas.Include(x => x.Personaje).Where(x => x.Personaje != null && x.Personaje.Faccion.Trim().ToUpper() == faccionNormalizada);
        if (poderMinimo.HasValue) consulta = consulta.Where(x => x.Poder >= poderMinimo);
        var cartas = string.Equals(orden, "desc", StringComparison.OrdinalIgnoreCase)
            ? consulta.OrderByDescending(x => x.Poder)
            : consulta.OrderBy(x => x.Poder);
        return Ok(new { data = await cartas.Select(x => new { x.Id, x.Nombre, x.Poder, personaje = x.Personaje!.Nombre }).ToListAsync() });
    }

    [HttpGet("{nombre}/eventos-ganados")]
    public async Task<IActionResult> Eventos(string nombre, DateTime? desde = null, DateTime? hasta = null)
    {
        if (desde.HasValue && hasta.HasValue && desde > hasta) return BadRequest(new { mensaje = "La fecha desde no puede ser posterior a hasta." });
        var faccionNormalizada = nombre.Trim().ToUpper();
        var consulta = db.Eventos.Where(x => x.FaccionGanadora.Trim().ToUpper() == faccionNormalizada);
        if (desde.HasValue) consulta = consulta.Where(x => x.Fecha >= desde);
        if (hasta.HasValue) consulta = consulta.Where(x => x.Fecha <= hasta);
        return Ok(new { data = await consulta.OrderByDescending(x => x.Fecha).ToListAsync() });
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
        var primera = estadisticas.FirstOrDefault(x => string.Equals(x.Faccion, faccionA.Trim(), StringComparison.OrdinalIgnoreCase));
        var segunda = estadisticas.FirstOrDefault(x => string.Equals(x.Faccion, faccionB.Trim(), StringComparison.OrdinalIgnoreCase));
        return primera is null || segunda is null ? NotFound() : Ok(new { faccionA = primera, faccionB = segunda, diferencias = new { personajes = primera.Personajes - segunda.Personajes, eventosGanados = primera.EventosGanados - segunda.EventosGanados } });
    }
}
