using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IClienteService
{
    Task<List<Cliente>> ObtenerClientesAsync();

    Task<Cliente?> ObtenerClientePorIdAsync(int id);

    Task CrearClienteAsync(Cliente cliente);
}
