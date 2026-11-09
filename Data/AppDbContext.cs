using Microsoft.EntityFrameworkCore;
namespace FacciStats;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Personaje> Personajes => Set<Personaje>();
    public DbSet<Carta> Cartas => Set<Carta>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personaje>(entity =>
        {
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Especie).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Faccion).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Afiliacion).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Estado).HasMaxLength(20).HasDefaultValue("desconocido").IsRequired();
            entity.Property(x => x.Image).HasMaxLength(500);
            entity.HasData(
                new Personaje { Id = 101, Nombre = "Luke Skywalker", Especie = "Humano", Faccion = "Rebelde", Afiliacion = "Orden Jedi y Alianza Rebelde", Estado = "vivo", FuerzaSensitivo = true, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/SWCA_-_Luke_Skywalker_(16580421784).jpg?width=500" },
                new Personaje { Id = 102, Nombre = "Darth Vader", Especie = "Humano", Faccion = "Imperio", Afiliacion = "Imperio Galáctico", Estado = "muerto", FuerzaSensitivo = true, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Darth_Vader_at_Galaxy%E2%80%99s_Edge.jpg?width=500" },
                new Personaje { Id = 103, Nombre = "Han Solo", Especie = "Humano", Faccion = "Rebelde", Afiliacion = "Alianza Rebelde", Estado = "vivo", FuerzaSensitivo = false, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Han_Solo_in_Carbonite_(31649435213).jpg?width=500" },
                new Personaje { Id = 104, Nombre = "Chewbacca", Especie = "Wookiee", Faccion = "Rebelde", Afiliacion = "Alianza Rebelde", Estado = "vivo", FuerzaSensitivo = false, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Chewbacca_(7865110592).jpg?width=500" },
                new Personaje { Id = 105, Nombre = "Yoda", Especie = "Desconocida", Faccion = "Neutral", Afiliacion = "Orden Jedi", Estado = "muerto", FuerzaSensitivo = true, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Yoda_I_know_you%27re_in_there%21.jpg?width=500" },
                new Personaje { Id = 106, Nombre = "Leia Organa", Especie = "Humana", Faccion = "Rebelde", Afiliacion = "Alianza Rebelde", Estado = "vivo", FuerzaSensitivo = false, Image = "https://commons.wikimedia.org/wiki/Special:Redirect/file/Leia_Organa_Solo_Skywalker.jpg?width=500" });
        });
        modelBuilder.Entity<Carta>(entity =>
        {
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.HasOne(x => x.Personaje).WithMany(x => x.Cartas).HasForeignKey(x => x.PersonajeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasData(
                new Carta { Id = 101, PersonajeId = 101, Nombre = "Carta de Luke", Poder = 92 },
                new Carta { Id = 102, PersonajeId = 102, Nombre = "Carta de Vader", Poder = 95 },
                new Carta { Id = 103, PersonajeId = 103, Nombre = "Carta de Han", Poder = 76 },
                new Carta { Id = 104, PersonajeId = 104, Nombre = "Carta de Chewbacca", Poder = 82 },
                new Carta { Id = 105, PersonajeId = 105, Nombre = "Carta de Yoda", Poder = 98 },
                new Carta { Id = 106, PersonajeId = 106, Nombre = "Carta de Leia", Poder = 88 });
        });
        modelBuilder.Entity<Evento>(entity =>
        {
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.FaccionGanadora).HasMaxLength(80).IsRequired();
            entity.HasData(
                new Evento { Id = 101, Nombre = "Batalla de Yavin", Fecha = new DateTime(2024, 1, 15), FaccionGanadora = "Rebelde" },
                new Evento { Id = 102, Nombre = "Batalla de Hoth", Fecha = new DateTime(2024, 3, 20), FaccionGanadora = "Imperio" },
                new Evento { Id = 103, Nombre = "Misión en Endor", Fecha = new DateTime(2024, 6, 10), FaccionGanadora = "Rebelde" },
                new Evento { Id = 104, Nombre = "Consejo de la Fuerza", Fecha = new DateTime(2024, 9, 5), FaccionGanadora = "Neutral" });
        });
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Telefono).HasMaxLength(255);
        });
    }
}
