
using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IDatosService
{
    // Clientes
    List<Cliente> ObtenerClientes();
    Cliente? ObtenerClientePorId(int id);
    void AgregarCliente(Cliente cliente);

    // Productos
    List<Producto> ObtenerProductos();
    Producto? ObtenerProductoPorId(int id);
    void AgregarProducto(Producto producto);

    // Pedidos
    List<Pedido> ObtenerPedidos();
    Pedido? ObtenerPedidoPorId(int id);
    void AgregarPedido(Pedido pedido);
    bool CambiarEstadoPedido(int pedidoId, int estadoId);
}
