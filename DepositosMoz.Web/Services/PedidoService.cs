using DepositosMoz.Web.Models;
using DepositosMoz.Web.Queries;
using DepositosMoz.Web.Repositories;

namespace DepositosMoz.Web.Services;

public class PedidoService : IPedidoService
{
    private readonly IRepositorio<Pedido> _repositorioPedido;
    private readonly ConsultasMoz _consultas;
    private readonly IClienteService _clienteService;
    private readonly IProductoService _productoService;
    private readonly IUnidadTrabajo _unidadTrabajo;

    public PedidoService(
        IRepositorio<Pedido> repositorioPedido,
        ConsultasMoz consultas,
        IClienteService clienteService,
        IProductoService productoService,
        IUnidadTrabajo unidadTrabajo)
    {
        _repositorioPedido = repositorioPedido;
        _consultas = consultas;
        _clienteService = clienteService;
        _productoService = productoService;
        _unidadTrabajo = unidadTrabajo;
    }

    public async Task<List<Pedido>> ObtenerPedidosAsync()
    {
        return await _consultas.PedidosAsync();
    }

    public async Task<Pedido?> ObtenerPedidoPorIdAsync(int id)
    {
        var pedidos =
            await _consultas.PedidosAsync();

        return pedidos.FirstOrDefault(
            pedido => pedido.Id == id);
    }

    public async Task CrearPedidoAsync(Pedido pedido)
    {
        ArgumentNullException.ThrowIfNull(pedido);

        await _unidadTrabajo.EjecutarEnTransaccionAsync(
            async () =>
            {
                var cliente =
                    await _clienteService.ObtenerClientePorIdAsync(
                        pedido.ClienteId);

                if (cliente == null || !cliente.Activo)
                {
                    throw new ArgumentException(
                        "Debe seleccionar un cliente válido y activo");
                }

                var fechaRegistro = DateTime.Now;

                if (pedido.FechaEntrega == default ||
                    pedido.FechaEntrega.Date < fechaRegistro.Date)
                {
                    throw new ArgumentException(
                        "La fecha de entrega debe ser posterior o igual a la fecha del pedido");
                }

                if (pedido.Detalles == null ||
                    pedido.Detalles.Count == 0)
                {
                    throw new ArgumentException(
                        "El pedido debe contener al menos un producto");
                }

                var productos =
                    new Dictionary<int, Producto>();

                foreach (var detalle in pedido.Detalles)
                {
                    if (detalle == null)
                    {
                        throw new ArgumentException(
                            "El pedido contiene un detalle inválido");
                    }

                    var producto =
                        await _productoService
                            .ObtenerProductoPorIdAsync(
                                detalle.ProductoId);

                    if (producto == null ||
                        !producto.Activo)
                    {
                        throw new ArgumentException(
                            "El pedido contiene un producto inválido");
                    }

                    if (detalle.Cantidad <= 0)
                    {
                        throw new ArgumentException(
                            "La cantidad debe ser mayor que cero");
                    }

                    productos[producto.Id] = producto;
                }

                var cantidadesPorProducto =
                    pedido.Detalles
                        .GroupBy(
                            detalle =>
                                detalle.ProductoId)
                        .Select(grupo => new
                        {
                            ProductoId = grupo.Key,
                            CantidadTotal =
                                grupo.Sum(
                                    detalle =>
                                        (long)detalle.Cantidad)
                        })
                        .ToList();

                foreach (var item in cantidadesPorProducto)
                {
                    var producto =
                        productos[item.ProductoId];

                    if (item.CantidadTotal >
                        producto.Stock)
                    {
                        throw new ArgumentException(
                            $"Stock insuficiente para {producto.Nombre}");
                    }
                }

                pedido.FechaPedido = fechaRegistro;
                pedido.EstadoPedidoId = 1;

                foreach (var detalle in pedido.Detalles)
                {
                    var producto =
                        productos[detalle.ProductoId];

                    detalle.PrecioUnitario =
                        producto.Precio;
                }

                await _repositorioPedido
                    .AgregarAsync(pedido);

                foreach (var item in cantidadesPorProducto)
                {
                    var producto =
                        productos[item.ProductoId];

                    producto.Stock -=
                        (int)item.CantidadTotal;

                    await _productoService
                        .ActualizarProductoAsync(
                            producto);
                }
            });
    }

    public async Task<bool> CambiarEstadoPedidoAsync(
        int id,
        int estadoId)
    {
        var pedido =
            await _repositorioPedido
                .ObtenerPorIdAsync(id);

        if (pedido == null)
        {
            return false;
        }

        pedido.EstadoPedidoId = estadoId;

        await _repositorioPedido
            .ActualizarAsync(pedido);

        return true;
    }
}
