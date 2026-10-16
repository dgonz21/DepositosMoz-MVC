
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
}

