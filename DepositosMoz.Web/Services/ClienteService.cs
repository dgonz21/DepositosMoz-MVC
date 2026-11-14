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
        ArgumentNullException.ThrowIfNull(cliente);

        if (string.IsNullOrWhiteSpace(cliente.Nombre))
        {
            throw new ArgumentException(
                "El nombre del cliente es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(cliente.Telefono))
        {
            throw new ArgumentException(
                "El teléfono del cliente es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(cliente.Direccion))
        {
            throw new ArgumentException(
                "La dirección del cliente es obligatoria");
        }

        cliente.Nombre = cliente.Nombre.Trim();
        cliente.Telefono = cliente.Telefono.Trim();
        cliente.Direccion = cliente.Direccion.Trim();
        cliente.Activo = true;

        await _repositorioCliente.AgregarAsync(cliente);
    }

    public async Task<bool> ActualizarClienteAsync(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        var clienteExistente =
            await _repositorioCliente.ObtenerPorIdAsync(cliente.Id);

        if (clienteExistente == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(cliente.Nombre) ||
            string.IsNullOrWhiteSpace(cliente.Telefono) ||
            string.IsNullOrWhiteSpace(cliente.Direccion))
        {
            throw new ArgumentException(
                "Todos los datos del cliente son obligatorios");
        }

        clienteExistente.Nombre = cliente.Nombre.Trim();
        clienteExistente.Telefono = cliente.Telefono.Trim();
        clienteExistente.Direccion = cliente.Direccion.Trim();

        await _repositorioCliente.ActualizarAsync(clienteExistente);

        return true;
    }

    public async Task<bool> DesactivarClienteAsync(int id)
    {
        var cliente =
            await _repositorioCliente.ObtenerPorIdAsync(id);

        if (cliente == null)
        {
            return false;
        }

        cliente.Activo = false;

        await _repositorioCliente.ActualizarAsync(cliente);

        return true;
    }
}
