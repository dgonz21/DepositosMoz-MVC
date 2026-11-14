using DepositosMoz.Web.Data;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Queries;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Tests;

public class ConsultasPersistenciaTests
{
    [Fact]
    public async Task Relaciones_Y_Consultas_Linq_Funcionan()
    {
        using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();

        var opciones = new DbContextOptionsBuilder<DepositosMozDbContext>()
            .UseSqlite(conexion)
            .Options;

        await using var db = new DepositosMozDbContext(opciones);
        await db.Database.EnsureCreatedAsync();

        var cliente = new Cliente
        {
            Nombre = "Cliente LINQ",
            Telefono = "77770000",
            Direccion = "San Salvador",
            Activo = true
        };

        var producto = new Producto
        {
            Nombre = "Producto LINQ",
            Precio = 12.50m,
            Stock = 20,
            Activo = true
        };

        db.Clientes.Add(cliente);
        db.Productos.Add(producto);
        await db.SaveChangesAsync();

        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            EstadoPedidoId = 1,
            FechaPedido = DateTime.Now,
            FechaEntrega = DateTime.Now.AddDays(2)
        };

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        db.DetallesPedido.Add(new DetallePedido
        {
            PedidoId = pedido.Id,
            ProductoId = producto.Id,
            Cantidad = 3,
            PrecioUnitario = 12.50m
        });

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var consultas = new ConsultasMoz(db);

        var clientes = await consultas.ClientesActivosAsync();
        Assert.Contains(clientes, c => c.Id == cliente.Id);

        var productos = await consultas.ProductosDisponiblesAsync();
        Assert.Contains(productos, p => p.Id == producto.Id);

        var pedidos = await consultas.PedidosPorClienteAsync(cliente.Id);
        Assert.Contains(pedidos, p => p.Id == pedido.Id);

        var porEstado = await consultas.PedidosPorEstadoAsync(1);
        Assert.Contains(porEstado, p => p.Id == pedido.Id);

        var agrupados = await consultas.PedidosAgrupadosPorEstadoAsync();
        Assert.True(agrupados.ContainsKey(1));
        Assert.Equal(1, agrupados[1]);

        var resumen = await consultas.ResumenPedidosAsync();
        Assert.NotEmpty(resumen);

        var reporte = await consultas.ReporteProductosVendidosAsync();
        Assert.NotEmpty(reporte);
    }
}
