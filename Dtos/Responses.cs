namespace FacciStats;
public record PersonajeDto(int Id, string Nombre, string Faccion, bool EstaMuerto, string? BiografiaSecreta);
public record FaccionDto(string Faccion, int Personajes, int Vivos, int Muertos, int Cartas, double? PoderPromedio, int EventosGanados, double PorcentajeMuertos);
