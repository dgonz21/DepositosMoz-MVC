using DepositosMoz.Web.Controllers;
using DepositosMoz.Web.Data;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Tests;

public class ProductosControllerTests
{
    [Fact]
    public async Task Index_RetornaVistaConListaDeProductos()
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
            new Repositorio<Producto>(db);

        var servicio =
            new ProductoService(repositorio);

        var controller =
            new ProductosController(servicio);

        var resultado =
            await controller.Index();

        var vista =
            Assert.IsType<ViewResult>(resultado);

        var modelo =
            Assert.IsAssignableFrom<IEnumerable<Producto>>(
                vista.Model);

        Assert.NotNull(modelo);
    }

    [Fact]
    public void Crear_Get_RetornaVista()
    {
        var controller =
            new ProductosController(null!);

        var resultado =
            controller.Crear();

        Assert.IsType<ViewResult>(resultado);
    }

    [Fact]
    public async Task Crear_Post_ModeloValido_AgregaProducto()
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
            new Repositorio<Producto>(db);

        var servicio =
            new ProductoService(repositorio);

        var controller =
            new ProductosController(servicio);

        var modelo = new ProductoCrearViewModel
        {
            Nombre = "Coca Cola",
            Descripcion = "Bebida gaseosa",
            Precio = 1.50m,
            Stock = 20
        };

        var resultado =
            await controller.Crear(modelo);

        var redireccion =
            Assert.IsType<RedirectToActionResult>(
                resultado);

        Assert.Equal(
            "Index",
            redireccion.ActionName);

        var productos =
            await repositorio.ObtenerTodosAsync();

        Assert.Contains(
            productos,
            producto =>
                producto.Nombre == "Coca Cola");
    }

    [Fact]
    public async Task Crear_Post_ModeloInvalido_RetornaVista()
    {
        var controller =
            new ProductosController(null!);

        var modelo = new ProductoCrearViewModel
        {
            Nombre = "",
            Descripcion = "",
            Precio = 0,
            Stock = 0
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
    public async Task Editar_Post_ActualizaProducto()
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
            new Repositorio<Producto>(db);

        var servicio =
            new ProductoService(repositorio);

        var producto = new Producto
        {
            Nombre = "Producto original",
            Descripcion = "Descripción original",
            Precio = 1.00m,
            Stock = 10,
            Activo = true
        };

        await repositorio.AgregarAsync(producto);

        var controller =
            new ProductosController(servicio);

        var modelo = new ProductoEditarViewModel
        {
            Id = producto.Id,
            Nombre = "Producto actualizado",
            Descripcion = "Descripción actualizada",
            Precio = 2.50m,
            Stock = 25
        };

        var resultado =
            await controller.Editar(
                producto.Id,
                modelo);

        Assert.IsType<RedirectToActionResult>(
            resultado);

        db.ChangeTracker.Clear();

        var actualizado =
            await repositorio.ObtenerPorIdAsync(
                producto.Id);

        Assert.NotNull(actualizado);

        Assert.Equal(
            "Producto actualizado",
            actualizado.Nombre);

        Assert.Equal(
            2.50m,
            actualizado.Precio);

        Assert.Equal(
            25,
            actualizado.Stock);
    }

    [Fact]
    public async Task Desactivar_ProductoExistente_LoDesactiva()
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
            new Repositorio<Producto>(db);

        var servicio =
            new ProductoService(repositorio);

        var producto = new Producto
        {
            Nombre = "Producto activo",
            Descripcion = "Producto de prueba",
            Precio = 1.25m,
            Stock = 15,
            Activo = true
        };

        await repositorio.AgregarAsync(producto);

        var controller =
            new ProductosController(servicio);

        var resultado =
            await controller.Desactivar(
                producto.Id);

        Assert.IsType<RedirectToActionResult>(
            resultado);

        db.ChangeTracker.Clear();

        var desactivado =
            await repositorio.ObtenerPorIdAsync(
                producto.Id);

        Assert.NotNull(desactivado);
        Assert.False(desactivado.Activo);
    }
}
