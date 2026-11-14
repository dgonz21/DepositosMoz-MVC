using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IClienteService
{
    Task<List<Cliente>> ObtenerClientesAsync();

    Task<Cliente?> ObtenerClientePorIdAsync(int id);

    Task CrearClienteAsync(Cliente cliente);

    Task<bool> ActualizarClienteAsync(Cliente cliente);

    Task<bool> DesactivarClienteAsync(int id);
}
