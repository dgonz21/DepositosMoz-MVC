using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Tests;

public class ClientesControllerTests
{
    [Fact]
    public void Index_RetornaVistaConListaDeClientes()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ClientesController(servicio);

        var resultado = controller.Index();

        var vista = Assert.IsType<ViewResult>(resultado);

        var modelo = Assert.IsAssignableFrom<IEnumerable<Cliente>>(
            vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public void Crear_Get_RetornaVista()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ClientesController(servicio);

        var resultado = controller.Crear();

        Assert.IsType<ViewResult>(resultado);
    }

    [Fact]
    public void Crear_Post_ModeloValido_AgregaCliente()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ClientesController(servicio);

        var modelo = new ClienteCrearViewModel
        {
            Nombre = "Cliente de prueba",
            Telefono = "7777-7777",
            Direccion = "San Salvador"
        };

        var resultado = controller.Crear(modelo);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(resultado);

        Assert.Equal("Index", redireccion.ActionName);

        var clientes = servicio.ObtenerClientes();

        Assert.Contains(
            clientes,
            cliente => cliente.Nombre == "Cliente de prueba");
    }

    [Fact]
    public void Crear_Post_ModeloInvalido_RetornaLaVista()
    {
        var servicio = new DatosMemoriaService();
        var controller = new ClientesController(servicio);

        var modelo = new ClienteCrearViewModel
        {
            Nombre = "",
            Telefono = "",
            Direccion = ""
        };

        controller.ModelState.AddModelError(
            "Nombre",
            "El nombre es obligatorio.");

        var resultado = controller.Crear(modelo);

        var vista = Assert.IsType<ViewResult>(resultado);

        Assert.Same(modelo, vista.Model);
    }
}