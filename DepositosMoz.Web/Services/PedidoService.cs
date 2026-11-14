using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;
using DepositosMoz.Web.Queries;

namespace DepositosMoz.Web.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorio<Pedido> _repositorioPedido;
    private readonly ConsultasMoz _consultas;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;

    public PedidoService(
        IRepositorio<Pedido> repositorioPedido,
        ConsultasMoz consultas,
        IClienteService clienteService,
        IProductoService productoService)
    {
        _repositorioPedido = repositorioPedido;
        _consultas = consultas;
        _clienteService = clienteService;
        _productoService = productoService;
    }

    public async Task<List<Pedido>> ObtenerPedidosAsync()
    {
        return await _consultas.PedidosAsync();
    }

    public async Task<Pedido?> ObtenerPedidoPorIdAsync(int id)
    {
    var pedidos = await _consultas.PedidosAsync();

    return pedidos.FirstOrDefault(
        pedido => pedido.Id == id);
    }

    public async Task CrearPedidoAsync(Pedido pedido)
    {
    ArgumentNullException.ThrowIfNull(pedido);

        var cliente =
            await _clienteService.ObtenerClientePorIdAsync(
                pedido.ClienteId);

        if (cliente == null || !cliente.Activo)
        {
            throw new ArgumentException(
                "Debe seleccionar un cliente válido y activo");
        }

                await _repositorioPedido.AgregarAsync(pedido);
        }

                public async Task<bool> CambiarEstadoPedidoAsync(int id, int estadoId)
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
