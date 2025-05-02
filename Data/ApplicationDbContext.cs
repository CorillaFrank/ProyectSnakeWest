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

    public DbSet<Producto> Productos { get; set; } // Nombre más estándar
    public DbSet<Proforma> Carritos { get; set;}

    public DbSet<Pago> Pagos { get; set;}
    public DbSet<Pedido> Pedidos { get; set;}
    public DbSet<DetallePedido> DetallePedidos { get; set;}
}
