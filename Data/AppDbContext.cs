using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Models;

namespace GreenDriveApi20260101.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bateria> Baterias { get; set; }
    public DbSet<EstacaoCarga> EstacoesCarga { get; set; }
    public DbSet<RegistroTelemetria> RegistrosTelemetria { get; set; }
    public DbSet<OrdemReciclagem> OrdensReciclagem { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bateria -> RegistroTelemetria (1:N)
        modelBuilder.Entity<RegistroTelemetria>()
            .HasOne(r => r.Bateria)
            .WithMany(b => b.Telemetrias)
            .HasForeignKey(r => r.BateriaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bateria -> OrdemReciclagem (1:N)
        modelBuilder.Entity<OrdemReciclagem>()
            .HasOne(o => o.Bateria)
            .WithMany(b => b.OrdensReciclagem)
            .HasForeignKey(o => o.BateriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // EstacaoCarga -> OrdemReciclagem (1:N)
        modelBuilder.Entity<OrdemReciclagem>()
            .HasOne(o => o.Estacao)
            .WithMany(e => e.OrdensReciclagem)
            .HasForeignKey(o => o.EstacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Precisão decimal para SQLite
        modelBuilder.Entity<OrdemReciclagem>()
            .Property(o => o.CustoProcessamento)
            .HasColumnType("TEXT");
    }
}
