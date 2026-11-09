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
            entity.Property(x => x.Faccion).HasMaxLength(80).IsRequired();
            entity.Property(x => x.BiografiaSecreta).HasColumnType("longtext");
        });
        modelBuilder.Entity<Carta>(entity =>
        {
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.HasOne(x => x.Personaje).WithMany(x => x.Cartas).HasForeignKey(x => x.PersonajeId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Evento>(entity =>
        {
            entity.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(x => x.FaccionGanadora).HasMaxLength(80).IsRequired();
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
