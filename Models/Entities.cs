namespace FacciStats;

public class Personaje { public int Id { get; set; } public string Nombre { get; set; } = ""; public string Faccion { get; set; } = ""; public bool EstaMuerto { get; set; } public string? BiografiaSecreta { get; set; } public List<Carta> Cartas { get; set; } = []; }
public class Carta { public int Id { get; set; } public int PersonajeId { get; set; } public string Nombre { get; set; } = ""; public int Poder { get; set; } public Personaje? Personaje { get; set; } }
public class Evento { public int Id { get; set; } public string Nombre { get; set; } = ""; public DateTime Fecha { get; set; } public string FaccionGanadora { get; set; } = ""; }
public class Usuario { public int Id { get; set; } public string Nombre { get; set; } = ""; public string Email { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string? Telefono { get; set; } }
