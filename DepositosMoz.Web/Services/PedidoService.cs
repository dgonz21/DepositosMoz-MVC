using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;

namespace DepositosMoz.Web.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorio<Pedido> _repositorioPedido;

    public PedidoService(IRepositorio<Pedido> repositorioPedido)
    {
        _repositorioPedido = repositorioPedido;
    }

    public async Task<List<Pedido>> ObtenerPedidosAsync()
    {
        return await _repositorioPedido.ObtenerTodosAsync();
    }

    public async Task<Pedido?> ObtenerPedidoPorIdAsync(int id)
    {
        return await _repositorioPedido.ObtenerPorIdAsync(id);
    }

    public async Task CrearPedidoAsync(Pedido pedido)
    {
        await _repositorioPedido.AgregarAsync(pedido);
    }

    public async Task<bool> CambiarEstadoPedidoAsync(
        int id,
        int estadoId)
    {
        var pedido = await _repositorioPedido.ObtenerPorIdAsync(id);

        if (pedido == null)
        {
            return false;
        }

        pedido.EstadoPedidoId = estadoId;

        await _repositorioPedido.ActualizarAsync(pedido);

        return true;
    }
}
