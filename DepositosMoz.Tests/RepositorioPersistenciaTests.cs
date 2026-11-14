using DepositosMoz.Web.Data;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Tests;

public class RepositorioPersistenciaTests
{
    [Fact]
    public async Task Repositorio_CRUD_Completo_Funciona()
    {
        using var conexion = new SqliteConnection("Data Source=:memory:");
        await conexion.OpenAsync();

        var opciones = new DbContextOptionsBuilder<DepositosMozDbContext>()
            .UseSqlite(conexion)
            .Options;

        await using var db = new DepositosMozDbContext(opciones);
        await db.Database.EnsureCreatedAsync();

        var repo = new Repositorio<Cliente>(db);

        // CREATE
        var cliente = new Cliente
        {
            Nombre = "Cliente de prueba",
            Telefono = "77770000",
            Direccion = "San Salvador",
            Activo = true
        };

        await repo.AgregarAsync(cliente);
        Assert.True(cliente.Id > 0);

        // READ
        var recuperado = await repo.ObtenerPorIdAsync(cliente.Id);
        Assert.NotNull(recuperado);
        Assert.Equal("Cliente de prueba", recuperado.Nombre);

        // LIST
        var todos = await repo.ObtenerTodosAsync();
        Assert.Contains(todos, c => c.Id == cliente.Id);

        // FILTER
        var filtrados = await repo.FiltrarAsync(c => c.Activo);
        Assert.Contains(filtrados, c => c.Id == cliente.Id);

        // UPDATE
        cliente.Nombre = "Cliente actualizado";
        await repo.ActualizarAsync(cliente);

        db.ChangeTracker.Clear();

        var actualizado = await repo.ObtenerPorIdAsync(cliente.Id);
        Assert.NotNull(actualizado);
        Assert.Equal("Cliente actualizado", actualizado.Nombre);

        // DELETE
        await repo.EliminarAsync(actualizado);

        db.ChangeTracker.Clear();

        var eliminado = await repo.ObtenerPorIdAsync(cliente.Id);
        Assert.Null(eliminado);
    }
}
