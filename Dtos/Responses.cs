namespace FacciStats;

public record PersonajeDto(int Id, string Nombre, string Especie, string Faccion, string Afiliacion, string Estado, bool FuerzaSensitivo, string Image);
public record FaccionDto(string Faccion, int Personajes, int Vivos, int Muertos, int Cartas, double? PoderPromedio, int EventosGanados, double PorcentajeMuertos);
