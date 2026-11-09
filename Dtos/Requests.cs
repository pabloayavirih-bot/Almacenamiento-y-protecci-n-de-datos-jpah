using System.ComponentModel.DataAnnotations;
namespace FacciStats;

public record PersonajeRequest(
    [Required, StringLength(100)] string Nombre,
    [Required, StringLength(80)] string Faccion,
    bool EstaMuerto,
    string? BiografiaSecreta);

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
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    string? Telefono);
