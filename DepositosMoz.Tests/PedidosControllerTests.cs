using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.DTOs;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DepositosMoz.Tests;

public class PedidosControllerTests
{
    [Fact]
    public void Index_RetornaVistaConListaDePedidos()
    {
        // Arrange
        var servicio = new DatosMemoriaService();
        var controller = new PedidosController(servicio);

        // Act
        var resultado = controller.Index();

        // Assert
        var vista = Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsAssignableFrom<IEnumerable<PedidoResumenDto>>(
                vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public void Crear_Get_RetornaVistaConModelo()
    {
        // Arrange
        var servicio = new DatosMemoriaService();
        var controller = new PedidosController(servicio);

        // Act
        var resultado = controller.Crear();

        // Assert
        var vista = Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsType<PedidoCrearViewModel>(
                vista.Model);

        Assert.NotNull(modelo.Detalles);
        Assert.Single(modelo.Detalles);
    }

    [Fact]
    public void Crear_Post_ModeloValido_AgregaPedido()
    {
        // Arrange
        var servicio = new DatosMemoriaService();

        var cliente = CrearCliente(servicio);
        var producto = CrearProducto(servicio);

        var controller = new PedidosController(servicio);

        var modelo = new PedidoCrearViewModel
        {
            ClienteId = cliente.Id,
            FechaEntrega = DateTime.Now.AddDays(1),
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

        var cantidadAntes =
            servicio.ObtenerPedidos().Count;

        // Act
        var resultado = controller.Crear(modelo);

        // Assert
        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Detalle",
            redireccion.ActionName);

        var cantidadDespues =
            servicio.ObtenerPedidos().Count;

        Assert.Equal(
            cantidadAntes + 1,
            cantidadDespues);
    }

    [Fact]
    public void Crear_Post_ModeloInvalido_RetornaVista()
    {
        // Arrange
        var servicio = new DatosMemoriaService();
        var controller = new PedidosController(servicio);

        var modelo = new PedidoCrearViewModel();

        controller.ModelState.AddModelError(
            "ClienteId",
            "El cliente es obligatorio.");

        // Act
        var resultado = controller.Crear(modelo);

        // Assert
        var vista =
            Assert.IsType<ViewResult>(
                resultado);

        Assert.Same(
            modelo,
            vista.Model);
    }

    [Fact]
    public void Detalle_PedidoExistente_RetornaVista()
    {
        // Arrange
        var servicio = new DatosMemoriaService();

        var cliente = CrearCliente(servicio);
        var producto = CrearProducto(servicio);

        var pedido = CrearPedido(
            servicio,
            cliente,
            producto);

        var controller =
            new PedidosController(servicio);

        // Act
        var resultado =
            controller.Detalle(pedido.Id);

        // Assert
        var vista =
            Assert.IsType<ViewResult>(
                resultado);

        var modelo =
            Assert.IsType<Pedido>(
                vista.Model);

        Assert.Equal(
            pedido.Id,
            modelo.Id);
    }

    [Fact]
    public void Detalle_PedidoNoExiste_RetornaNotFound()
    {
        // Arrange
        var servicio = new DatosMemoriaService();
        var controller = new PedidosController(servicio);

        // Act
        var resultado =
            controller.Detalle(99999);

        // Assert
        Assert.IsType<NotFoundResult>(
            resultado);
    }

    [Fact]
    public void CambiarEstado_EstadoValido_ActualizaPedido()
    {
        // Arrange
        var servicio = new DatosMemoriaService();

        var cliente = CrearCliente(servicio);
        var producto = CrearProducto(servicio);

        var pedido = CrearPedido(
            servicio,
            cliente,
            producto);

        var controller =
            new PedidosController(servicio);

        controller.TempData =
            new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProviderFalso());

        // Act
        var resultado =
            controller.CambiarEstado(
                pedido.Id,
                2);

        // Assert
        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Detalle",
            redireccion.ActionName);

        var pedidoActualizado =
            servicio.ObtenerPedidoPorId(
                pedido.Id);

        Assert.NotNull(pedidoActualizado);

        Assert.Equal(
            2,
            pedidoActualizado.EstadoPedidoId);
    }

    [Fact]
    public void CambiarEstado_EstadoInvalido_RetornaBadRequest()
    {
        // Arrange
        var servicio = new DatosMemoriaService();

        var cliente = CrearCliente(servicio);
        var producto = CrearProducto(servicio);

        var pedido = CrearPedido(
            servicio,
            cliente,
            producto);

        var controller =
            new PedidosController(servicio);

        // Act
        var resultado =
            controller.CambiarEstado(
                pedido.Id,
                10);

        // Assert
        Assert.IsType<BadRequestObjectResult>(
            resultado);
    }

    private static Cliente CrearCliente(
        DatosMemoriaService servicio)
    {
        var cliente = new Cliente
        {
            Nombre = "Cliente prueba",
            Telefono = "7777-7777",
            Direccion = "San Salvador"
        };

        servicio.AgregarCliente(cliente);

        return cliente;
    }

    private static Producto CrearProducto(
        DatosMemoriaService servicio)
    {
        var producto = new Producto
        {
            Nombre = "Producto prueba",
            Descripcion = "Producto para pruebas",
            Precio = 2.50m,
            Stock = 20
        };

        servicio.AgregarProducto(producto);

        return producto;
    }

    private static Pedido CrearPedido(
        DatosMemoriaService servicio,
        Cliente cliente,
        Producto producto)
    {
        var pedido = new Pedido
        {
            ClienteId = cliente.Id,
            FechaEntrega = DateTime.Now.AddDays(1),
            Observaciones = "Pedido para pruebas",

            Detalles = new List<DetallePedido>
            {
                new DetallePedido
                {
                    ProductoId = producto.Id,
                    Cantidad = 2
                }
            }
        };

        servicio.AgregarPedido(pedido);

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