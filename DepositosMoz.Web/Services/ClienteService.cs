using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;

namespace DepositosMoz.Web.Services;

public class ClienteService : IClienteService
{
    private readonly IRepositorio<Cliente> _repositorioCliente;

    public ClienteService(IRepositorio<Cliente> repositorioCliente)
    {
        _repositorioCliente = repositorioCliente;
    }

    public async Task<List<Cliente>> ObtenerClientesAsync()
    {
        return await _repositorioCliente.ObtenerTodosAsync();
    }

    public async Task<Cliente?> ObtenerClientePorIdAsync(int id)
    {
        return await _repositorioCliente.ObtenerPorIdAsync(id);
    }

    public async Task CrearClienteAsync(Cliente cliente)
    {
        await _repositorioCliente.AgregarAsync(cliente);
    }
}
