using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IPedidoService
{
    Task<List<Pedido>> ObtenerPedidosAsync();

    Task<Pedido?> ObtenerPedidoPorIdAsync(int id);

    Task CrearPedidoAsync(Pedido pedido);

    Task<bool> CambiarEstadoPedidoAsync(
        int id,
        int estadoId);
}
