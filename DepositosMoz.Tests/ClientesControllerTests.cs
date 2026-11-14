using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.Data;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Tests;

public class ClientesControllerTests
{
    [Fact]
    public async Task Index_RetornaVistaConListaDeClientes()
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

        var repositorio =
            new Repositorio<Cliente>(db);

        var servicio =
            new ClienteService(repositorio);

        var controller =
            new ClientesController(servicio);

        var resultado =
            await controller.Index();

        var vista =
            Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsAssignableFrom<IEnumerable<Cliente>>(
                vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public void Crear_Get_RetornaVista()
    {
        var controller =
            new ClientesController(null!);

        var resultado =
            controller.Crear();

        Assert.IsType<ViewResult>(resultado);
    }

    [Fact]
    public async Task Crear_Post_ModeloValido_AgregaCliente()
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

        var repositorio =
            new Repositorio<Cliente>(db);

        var servicio =
            new ClienteService(repositorio);

        var controller =
            new ClientesController(servicio);

        var modelo = new ClienteCrearViewModel
        {
            Nombre = "Cliente de prueba",
            Telefono = "7777-7777",
            Direccion = "San Salvador"
        };

        var resultado =
            await controller.Crear(modelo);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Index",
            redireccion.ActionName);

        var clientes =
            await repositorio.ObtenerTodosAsync();

        Assert.Contains(
            clientes,
            cliente =>
                cliente.Nombre == "Cliente de prueba");
    }

    [Fact]
    public async Task Crear_Post_ModeloInvalido_RetornaVista()
    {
        var controller =
            new ClientesController(null!);

        var modelo = new ClienteCrearViewModel
        {
            Nombre = "",
            Telefono = "",
            Direccion = ""
        };

        controller.ModelState.AddModelError(
            "Nombre",
            "El nombre es obligatorio");

        var resultado =
            await controller.Crear(modelo);

        var vista =
            Assert.IsType<ViewResult>(resultado);

        Assert.Same(
            modelo,
            vista.Model);
    }

    [Fact]
    public async Task Editar_Post_ActualizaCliente()
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

        var repositorio =
            new Repositorio<Cliente>(db);

        var servicio =
            new ClienteService(repositorio);

        var cliente = new Cliente
        {
            Nombre = "Cliente original",
            Telefono = "7777-7777",
            Direccion = "San Salvador",
            Activo = true
        };

        await repositorio.AgregarAsync(cliente);

        var controller =
            new ClientesController(servicio);

        var modelo = new ClienteEditarViewModel
        {
            Id = cliente.Id,
            Nombre = "Cliente actualizado",
            Telefono = "7777-8888",
            Direccion = "Santa Tecla"
        };

        var resultado =
            await controller.Editar(
                cliente.Id,
                modelo);

        Assert.IsType<RedirectToActionResult>(
            resultado);

        db.ChangeTracker.Clear();

        var actualizado =
            await repositorio.ObtenerPorIdAsync(
                cliente.Id);

        Assert.NotNull(actualizado);

        Assert.Equal(
            "Cliente actualizado",
            actualizado.Nombre);
    }

    [Fact]
    public async Task Desactivar_ClienteExistente_LoDesactiva()
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

        var repositorio =
            new Repositorio<Cliente>(db);

        var servicio =
            new ClienteService(repositorio);

        var cliente = new Cliente
        {
            Nombre = "Cliente activo",
            Telefono = "7777-7777",
            Direccion = "San Salvador",
            Activo = true
        };

        await repositorio.AgregarAsync(cliente);

        var controller =
            new ClientesController(servicio);

        controller.TempData =
            new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProviderFalso());

        var resultado =
            await controller.Desactivar(
                cliente.Id);

        Assert.IsType<RedirectToActionResult>(
            resultado);

        db.ChangeTracker.Clear();

        var desactivado =
            await repositorio.ObtenerPorIdAsync(
                cliente.Id);

        Assert.NotNull(desactivado);
        Assert.False(desactivado.Activo);
    }

    private class TempDataProviderFalso : ITempDataProvider
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
