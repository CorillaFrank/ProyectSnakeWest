using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyect_Snake_West.Models;

namespace Proyect_Snake_West.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Producto> Productos { get; set; }
    public DbSet<Proforma> Carritos { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallePedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // --- Pedido -> public.t_order ---
    modelBuilder.Entity<Pedido>(b =>
    {
        b.ToTable("t_order", "public");
        b.HasKey(x => x.ID);

        b.Property(x => x.ID).HasColumnName("id");
        b.Property(x => x.UserID).HasColumnName("UserID");   // exacto
        b.Property(x => x.Total).HasColumnName("Total");     // exacto
        b.Property(x => x.Status).HasColumnName("Status");   // exacto (mayúscula)
        // b.Property(x => x.PagoId).HasColumnName("pagoId"); // si existe en tu modelo
    });

    // --- DetallePedido -> public.t_order_detail ---
    modelBuilder.Entity<DetallePedido>(b =>
    {
        b.ToTable("t_order_detail", "public");
        b.HasKey(x => x.ID);

        b.Property(x => x.ID).HasColumnName("id");
        b.Property(x => x.ProductoId).HasColumnName("ProductoId"); // exacto
        b.Property(x => x.Cantidad).HasColumnName("Cantidad");
        b.Property(x => x.Precio).HasColumnName("Precio");
        b.Property(x => x.PedidoID).HasColumnName("pedidoID");     // exacto

        b.HasOne(x => x.Pedido)
         .WithMany(p => p.Detalles)
         .HasForeignKey(x => x.PedidoID);
    });
}

}
