using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.Data;
using DepositosMoz.Web.DTOs;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Queries;
using DepositosMoz.Web.Repositories;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Tests;

public class PedidosControllerTests
{
    [Fact]
    public async Task Index_RetornaVistaConListaDePedidos()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var controller = CrearController(db);

        var resultado =
            await controller.Index();

        var vista =
            Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsAssignableFrom<IEnumerable<PedidoResumenDto>>(
                vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public async Task Crear_Get_RetornaVistaConModelo()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var controller = CrearController(db);

        var resultado =
            await controller.Crear();

        var vista =
            Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsType<PedidoCrearViewModel>(
                vista.Model);

        Assert.NotNull(modelo.Detalles);
        Assert.Single(modelo.Detalles);
    }

    [Fact]
    public async Task Crear_Post_ModeloValido_AgregaPedido()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var cliente =
            await CrearClienteAsync(db);

        var producto =
            await CrearProductoAsync(db);

        var controller = CrearController(db);

        var modelo = new PedidoCrearViewModel
        {
            ClienteId = cliente.Id,
            FechaEntrega = DateTime.Today.AddDays(1),
            Observaciones = "Pedido de prueba",

            Detalles = new List<DetallePedidoCrearViewModel>
            {
                new DetallePedidoCrearViewModel
                {
                    ProductoId = producto.Id,
                    Cantidad = 2
                }
            }
        };

        var resultado =
            await controller.Crear(modelo);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Detalle",
            redireccion.ActionName);

        var pedido =
            await db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync();

        Assert.NotNull(pedido);
        Assert.Single(pedido.Detalles);
        Assert.Equal(1, pedido.EstadoPedidoId);

        Assert.Equal(
            producto.Precio,
            pedido.Detalles[0].PrecioUnitario);

        db.ChangeTracker.Clear();

        var productoActualizado =
            await db.Productos
                .AsNoTracking()
                .FirstAsync(
                    p => p.Id == producto.Id);

        Assert.Equal(
            18,
            productoActualizado.Stock);
    }

    [Fact]
    public async Task Crear_Post_StockAgotado_RechazaSegundoPedido()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var clienteUno =
            await CrearClienteAsync(db);

        var clienteDos = new Cliente
        {
            Nombre = "Segundo cliente",
            Telefono = "7777-8888",
            Direccion = "Santa Tecla",
            Activo = true
        };

        db.Clientes.Add(clienteDos);
        await db.SaveChangesAsync();

        var producto =
            await CrearProductoAsync(db);

        producto.Stock = 1;

        db.Productos.Update(producto);
        await db.SaveChangesAsync();

        var controller = CrearController(db);

        var primerPedido =
            new PedidoCrearViewModel
            {
                ClienteId = clienteUno.Id,
                FechaEntrega =
                    DateTime.Today.AddDays(1),

                Observaciones =
                    "Primer pedido",

                Detalles =
                    new List<DetallePedidoCrearViewModel>
                    {
                        new DetallePedidoCrearViewModel
                        {
                            ProductoId =
                                producto.Id,

                            Cantidad = 1
                        }
                    }
            };

        var resultadoPrimerPedido =
            await controller.Crear(
                primerPedido);

        Assert.IsType<RedirectToActionResult>(
            resultadoPrimerPedido);

        db.ChangeTracker.Clear();

        var productoSinStock =
            await db.Productos
                .AsNoTracking()
                .FirstAsync(
                    p => p.Id == producto.Id);

        Assert.Equal(
            0,
            productoSinStock.Stock);

        var segundoPedido =
            new PedidoCrearViewModel
            {
                ClienteId = clienteDos.Id,
                FechaEntrega =
                    DateTime.Today.AddDays(1),

                Observaciones =
                    "Segundo pedido",

                Detalles =
                    new List<DetallePedidoCrearViewModel>
                    {
                        new DetallePedidoCrearViewModel
                        {
                            ProductoId =
                                producto.Id,

                            Cantidad = 1
                        }
                    }
            };

        var resultadoSegundoPedido =
            await controller.Crear(
                segundoPedido);

        Assert.IsType<ViewResult>(
            resultadoSegundoPedido);

        Assert.False(
            controller.ModelState.IsValid);

        var cantidadPedidos =
            await db.Pedidos.CountAsync();

        Assert.Equal(
            1,
            cantidadPedidos);

        var stockFinal =
            await db.Productos
                .AsNoTracking()
                .Where(
                    p => p.Id == producto.Id)
                .Select(
                    p => p.Stock)
                .FirstAsync();

        Assert.Equal(
            0,
            stockFinal);
    }

    [Fact]
    public async Task Crear_Post_ModeloInvalido_RetornaVista()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var controller = CrearController(db);

        var modelo =
            new PedidoCrearViewModel();

        controller.ModelState.AddModelError(
            "ClienteId",
            "El cliente es obligatorio");

        var resultado =
            await controller.Crear(modelo);

        var vista =
            Assert.IsType<ViewResult>(
                resultado);

        Assert.Same(
            modelo,
            vista.Model);
    }

    [Fact]
    public async Task Detalle_PedidoExistente_RetornaVista()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var cliente =
            await CrearClienteAsync(db);

        var producto =
            await CrearProductoAsync(db);

        var pedido =
            await CrearPedidoAsync(
                db,
                cliente,
                producto);

        var controller = CrearController(db);

        var resultado =
            await controller.Detalle(
                pedido.Id);

        var vista =
            Assert.IsType<ViewResult>(
                resultado);

        var modelo =
            Assert.IsType<Pedido>(
                vista.Model);

        Assert.Equal(
            pedido.Id,
            modelo.Id);

        Assert.Single(
            modelo.Detalles);
    }

    [Fact]
    public async Task Detalle_PedidoNoExiste_RetornaNotFound()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var controller = CrearController(db);

        var resultado =
            await controller.Detalle(
                99999);

        Assert.IsType<NotFoundResult>(
            resultado);
    }

    [Fact]
    public async Task CambiarEstado_EstadoValido_ActualizaPedido()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var cliente =
            await CrearClienteAsync(db);

        var producto =
            await CrearProductoAsync(db);

        var pedido =
            await CrearPedidoAsync(
                db,
                cliente,
                producto);

        var controller = CrearController(db);

        controller.TempData =
            new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProviderFalso());

        var resultado =
            await controller.CambiarEstado(
                pedido.Id,
                2);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Detalle",
            redireccion.ActionName);

        db.ChangeTracker.Clear();

        var actualizado =
            await db.Pedidos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == pedido.Id);

        Assert.NotNull(actualizado);

        Assert.Equal(
            2,
            actualizado.EstadoPedidoId);
    }

    [Fact]
    public async Task CambiarEstado_EstadoInvalido_RetornaBadRequest()
    {
        using var conexion =
            new SqliteConnection("Data Source=:memory:");

        await conexion.OpenAsync();

        var opciones =
            new DbContextOptionsBuilder<DepositosMozDbContext>()
                .UseSqlite(conexion)
                .Options;

        await using var db =
            new DepositosMozDbContext(opciones);

        await db.Database.EnsureCreatedAsync();

        var cliente =
            await CrearClienteAsync(db);

        var producto =
            await CrearProductoAsync(db);

        var pedido =
            await CrearPedidoAsync(
                db,
                cliente,
                producto);

        var controller =
            CrearController(db);

        var resultado =
            await controller.CambiarEstado(
                pedido.Id,
                10);

        Assert.IsType<BadRequestObjectResult>(
            resultado);
    }

    private static PedidosController CrearController(
        DepositosMozDbContext db)
    {
        var repositorioCliente =
            new Repositorio<Cliente>(db);

        var repositorioProducto =
            new Repositorio<Producto>(db);

        var repositorioPedido =
            new Repositorio<Pedido>(db);

        var clienteService =
            new ClienteService(
                repositorioCliente);

        var productoService =
            new ProductoService(
                repositorioProducto);

        var consultas =
            new ConsultasMoz(db);

        var unidadTrabajo =
            new UnidadTrabajo(db);

        var pedidoService =
            new PedidoService(
                repositorioPedido,
                consultas,
                clienteService,
                productoService,
                unidadTrabajo);

        return new PedidosController(
            pedidoService,
            clienteService,
            productoService);
    }

    private static async Task<Cliente> CrearClienteAsync(
        DepositosMozDbContext db)
    {
        var repositorio =
            new Repositorio<Cliente>(db);

        var cliente = new Cliente
        {
            Nombre = "Cliente prueba",
            Telefono = "7777-7777",
            Direccion = "San Salvador",
            Activo = true
        };

        await repositorio.AgregarAsync(
            cliente);

        return cliente;
    }

    private static async Task<Producto> CrearProductoAsync(
        DepositosMozDbContext db)
    {
        var repositorio =
            new Repositorio<Producto>(db);

        var producto = new Producto
        {
            Nombre = "Producto prueba",
            Descripcion =
                "Producto para pruebas",
            Precio = 2.50m,
            Stock = 20,
            Activo = true
        };

        await repositorio.AgregarAsync(
            producto);

        return producto;
    }

    private static async Task<Pedido> CrearPedidoAsync(
        DepositosMozDbContext db,
        Cliente cliente,
        Producto producto)
    {
        var repositorioCliente =
            new Repositorio<Cliente>(db);

        var repositorioProducto =
            new Repositorio<Producto>(db);

        var repositorioPedido =
            new Repositorio<Pedido>(db);

        var clienteService =
            new ClienteService(
                repositorioCliente);

        var productoService =
            new ProductoService(
                repositorioProducto);

        var consultas =
            new ConsultasMoz(db);

        var unidadTrabajo =
            new UnidadTrabajo(db);

        var pedidoService =
            new PedidoService(
                repositorioPedido,
                consultas,
                clienteService,
                productoService,
                unidadTrabajo);

        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            FechaEntrega =
                DateTime.Today.AddDays(1),

            Observaciones =
                "Pedido para pruebas",

            Detalles =
                new List<DetallePedido>
                {
                    new DetallePedido
                    {
                        ProductoId =
                            producto.Id,

                        Cantidad = 2
                    }
                }
        };

        await pedidoService.CrearPedidoAsync(
            pedido);

        return pedido;
    }

    private class TempDataProviderFalso
        : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(
            HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public void SaveTempData(
            HttpContext context,
            IDictionary<string, object> values)
        {
        }
    }
}
