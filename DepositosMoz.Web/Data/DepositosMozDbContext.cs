using DepositosMoz.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Web.Data;

public class DepositosMozDbContext : DbContext
{
    public DepositosMozDbContext(
        DbContextOptions<DepositosMozDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<EstadoPedido> EstadosPedido => Set<EstadoPedido>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>()
            .HasMany<Pedido>()
            .WithOne()
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EstadoPedido>()
            .HasMany<Pedido>()
            .WithOne()
            .HasForeignKey(p => p.EstadoPedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Detalles)
            .WithOne()
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Producto>()
            .HasMany<DetallePedido>()
            .WithOne()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<DetallePedido>()
            .Property(d => d.PrecioUnitario)
            .HasPrecision(10, 2);

        modelBuilder.Entity<EstadoPedido>().HasData(
            new EstadoPedido
            {
                Id = 1,
                Nombre = "Pendiente"
            },
            new EstadoPedido
            {
                Id = 2,
                Nombre = "En preparación"
            },
            new EstadoPedido
            {
                Id = 3,
                Nombre = "Listo para entrega"
            },
            new EstadoPedido
            {
                Id = 4,
                Nombre = "Entregado"
            }
        );
    }
}
