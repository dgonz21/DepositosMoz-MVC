using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Tests;

public class ProductosControllerTests
{
    [Fact]
    public void Index_RetornaVistaConListaDeProductos()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ProductosController(servicio);

        var resultado = controller.Index();

        var vista = Assert.IsType<ViewResult>(resultado);

        var modelo = Assert.IsAssignableFrom<IEnumerable<Producto>>(
            vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public void Crear_Get_RetornaVista()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ProductosController(servicio);

        var resultado = controller.Crear();

        Assert.IsType<ViewResult>(resultado);
    }

    [Fact]
    public void Crear_Post_ModeloValido_AgregaProducto()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ProductosController(servicio);

        var modelo = new ProductoCrearViewModel
        {
            Nombre = "Coca Cola",
            Descripcion = "Bebida gaseosa",
            Precio = 1.50m,
            Stock = 20
        };

        var resultado = controller.Crear(modelo);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(resultado);

        Assert.Equal("Index", redireccion.ActionName);

        var productos = servicio.ObtenerProductos();

        Assert.Contains(
            productos,
            producto => producto.Nombre == "Coca Cola");
    }

    [Fact]
    public void Crear_Post_ModeloInvalido_RetornaLaVista()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ProductosController(servicio);

        var modelo = new ProductoCrearViewModel
        {
            Nombre = "",
            Descripcion = "",
            Precio = 0,
            Stock = 0
        };

        controller.ModelState.AddModelError(
            "Nombre",
            "El nombre es obligatorio.");

        var resultado = controller.Crear(modelo);

        var vista = Assert.IsType<ViewResult>(resultado);

        Assert.Same(modelo, vista.Model);
    }
}