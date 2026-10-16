
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using Xunit;

namespace DepositosMoz.Tests;

public class UnitTest1
{
    [Fact]
    public void AgregarCliente_DebeGuardarClienteCorrectamente()
    {
        var servicio = new DatosMemoriaService();

        var cliente = new Cliente
        {
            Nombre = "Juan Perez",
            Telefono = "7777-8888",
            Direccion = "San Salvador"
        };

        servicio.AgregarCliente(cliente);

        var clientes = servicio.ObtenerClientes();

        Assert.Single(clientes);
        Assert.Equal(1, clientes[0].Id);
        Assert.Equal("Juan Perez", clientes[0].Nombre);
    }

[Fact]
public void AgregarProducto_DebeGuardarProductoCorrectamente()
{
    // Arrange: preparamos los datos
    var servicio = new DatosMemoriaService();

    var producto = new Producto
    {
        Nombre = "Coca-Cola 2 litros",
        Descripcion = "Bebida gaseosa",
        Precio = 2.50m,
        Stock = 20
    };

    // Act: ejecutamos el método
    servicio.AgregarProducto(producto);

    // Assert: verificamos el resultado
    var productos = servicio.ObtenerProductos();

    Assert.Single(productos);
    Assert.Equal(1, productos[0].Id);
    Assert.Equal("Coca-Cola 2 litros", productos[0].Nombre);
    Assert.Equal(2.50m, productos[0].Precio);
    Assert.Equal(20, productos[0].Stock);
}
}

