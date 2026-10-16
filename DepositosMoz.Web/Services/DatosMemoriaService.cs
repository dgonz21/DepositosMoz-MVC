
using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public class DatosMemoriaService : IDatosService
{
    private readonly List<Cliente> _clientes = new();
    private readonly List<Producto> _productos = new();
    private readonly List<Pedido> _pedidos = new();

    private int _siguienteClienteId = 1;
    private int _siguienteProductoId = 1;
    private int _siguientePedidoId = 1;

    // CLIENTES

    public List<Cliente> ObtenerClientes()
    {
        return _clientes.ToList();
    }

    public Cliente? ObtenerClientePorId(int id)
    {
        return _clientes.FirstOrDefault(c => c.Id == id);
    }

    
public void AgregarCliente(Cliente cliente)
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

    cliente.Id = _siguienteClienteId++;
    _clientes.Add(cliente);
}


    // PRODUCTOS

    public List<Producto> ObtenerProductos()
    {
        return _productos.ToList();
    }

    public Producto? ObtenerProductoPorId(int id)
    {
        return _productos.FirstOrDefault(p => p.Id == id);
    }

    
public void AgregarProducto(Producto producto)
{
    ArgumentNullException.ThrowIfNull(producto);

    if (string.IsNullOrWhiteSpace(producto.Nombre))
    {
        throw new ArgumentException(
            "El nombre del producto es obligatorio");
    }

    if (producto.Nombre.Trim().Length > 100)
    {
        throw new ArgumentException(
            "El nombre no puede superar 100 caracteres");
    }

    if (producto.Descripcion?.Length > 250)
    {
        throw new ArgumentException(
            "La descripción no puede superar 250 caracteres");
    }

    if (producto.Precio <= 0 || producto.Precio > 999999.99m)
    {
        throw new ArgumentException(
            "El precio debe ser mayor que cero y estar dentro del límite permitido");
    }

    if (producto.Stock < 0)
    {
        throw new ArgumentException(
            "El stock no puede ser negativo");
    }

    producto.Nombre = producto.Nombre.Trim();
    producto.Descripcion = producto.Descripcion?.Trim();

    producto.Id = _siguienteProductoId++;
    _productos.Add(producto);
}


    // PEDIDOS

    public List<Pedido> ObtenerPedidos()
    {
        return _pedidos.ToList();
    }

    public Pedido? ObtenerPedidoPorId(int id)
    {
        return _pedidos.FirstOrDefault(p => p.Id == id);
    }

    
public void AgregarPedido(Pedido pedido)
{
    ArgumentNullException.ThrowIfNull(pedido);

    var cliente = ObtenerClientePorId(pedido.ClienteId);

    if (cliente == null || !cliente.Activo)
    {
        throw new ArgumentException(
            "Debe seleccionar un cliente válido y activo");
    }

    
var fechaRegistro = DateTime.Now;

if (pedido.FechaEntrega == default ||
    pedido.FechaEntrega < fechaRegistro)
{
    throw new ArgumentException(
        "La fecha de entrega debe ser posterior o igual a la fecha del pedido");
}


    if (pedido.Detalles == null || pedido.Detalles.Count == 0)
    {
        throw new ArgumentException(
            "El pedido debe contener al menos un producto");
    }

    foreach (var detalle in pedido.Detalles)
    {
        if (detalle == null)
        {
            throw new ArgumentException(
                "El pedido contiene un detalle inválido");
        }

        var producto = ObtenerProductoPorId(detalle.ProductoId);

        if (producto == null || !producto.Activo)
        {
            throw new ArgumentException(
                "El pedido contiene un producto inválido");
        }

        if (detalle.Cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero");
        }
    }

        
var cantidadesPorProducto = pedido.Detalles
    .GroupBy(d => d.ProductoId)
    .Select(grupo => new
    {
        ProductoId = grupo.Key,
        CantidadTotal = grupo.Sum(d => (long)d.Cantidad)
    });

foreach (var item in cantidadesPorProducto)
{
    var producto = ObtenerProductoPorId(item.ProductoId)!;

    if (item.CantidadTotal > producto.Stock)
    {
        throw new ArgumentException(
            $"Stock insuficiente para {producto.Nombre}");
    }
}

    pedido.FechaPedido = fechaRegistro;
    pedido.EstadoPedidoId = 1;

    foreach (var detalle in pedido.Detalles)
    {
        var producto = ObtenerProductoPorId(detalle.ProductoId)!;

        detalle.PrecioUnitario = producto.Precio;
        detalle.PedidoId = _siguientePedidoId;
    }

    pedido.Id = _siguientePedidoId++;
    _pedidos.Add(pedido);
}

public bool CambiarEstadoPedido(int pedidoId, int estadoId)
{
    var pedido = ObtenerPedidoPorId(pedidoId);

    if (pedido == null)
    {
        return false;
    }

    if (estadoId < 1 || estadoId > 4)
    {
        return false;
    }

    pedido.EstadoPedidoId = estadoId;
    return true;
}

}
