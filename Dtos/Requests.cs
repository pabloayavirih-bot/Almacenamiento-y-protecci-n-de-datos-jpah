using System.ComponentModel.DataAnnotations;
namespace FacciStats;

public record PersonajeRequest(
    [Required, StringLength(100)] string Nombre,
    [Required, StringLength(80)] string Especie,
    [Required, StringLength(80)] string Faccion,
    [Required, StringLength(120)] string Afiliacion,
    [Required, StringLength(20), RegularExpression("^(vivo|muerto|desconocido)$", ErrorMessage = "El estado debe ser vivo, muerto o desconocido.")] string Estado,
    bool FuerzaSensitivo,
    [Url, StringLength(500)] string? Image);

public record CartaRequest(
    int PersonajeId,
    [Required, StringLength(100)] string Nombre,
    [Range(0, 100000)] int Poder);

public record EventoRequest(
    [Required, StringLength(120)] string Nombre,
    DateTime Fecha,
    [Required, StringLength(80)] string FaccionGanadora);

public record UsuarioRequest(
    [Required] string Nombre,
    [Required, EmailAddress, StringLength(150)] string Email,
    [Required, MinLength(8)] string Password,
    [StringLength(30)] string? Telefono);
