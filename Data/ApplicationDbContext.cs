using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Models;

namespace Proyect_Snake_West.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Cambia esto para que coincida con lo que usas en el controlador
    public DbSet<Producto> Productos { get; set; } // Nombre más estándar


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuración específica
        modelBuilder.Entity<Producto>(entity => 
        {
            entity.ToTable("t_producto"); // Asegúrate que coincida con tu tabla real
        });
    }
}
