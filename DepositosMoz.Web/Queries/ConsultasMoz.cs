using DepositosMoz.Web.Data;
using DepositosMoz.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Web.Queries;

public class ConsultasMoz
{
    private readonly DepositosMozDbContext _db;

    public ConsultasMoz(DepositosMozDbContext db)
    {
        _db = db;
    }

    // 4.1 Consultas simples
    public Task<List<Cliente>> ClientesAsync() =>
        _db.Clientes.AsNoTracking().ToListAsync();

    public Task<List<Producto>> ProductosAsync() =>
        _db.Productos.AsNoTracking().ToListAsync();

    public Task<List<Pedido>> PedidosAsync() =>
        _db.Pedidos.AsNoTracking()
            .Include(p => p.Detalles).ToListAsync();

    public Task<List<EstadoPedido>> EstadosAsync() =>
        _db.EstadosPedido.AsNoTracking().ToListAsync();

    public Task<List<DetallePedido>> DetallesAsync() =>
        _db.DetallesPedido.AsNoTracking().ToListAsync();

    // 4.2 Filtros
    public Task<List<Cliente>> ClientesActivosAsync() =>
        _db.Clientes.Where(c => c.Activo).ToListAsync();

    public Task<List<Producto>> ProductosDisponiblesAsync() =>
        _db.Productos
            .Where(p => p.Activo && p.Stock > 0)
            .ToListAsync();

    public Task<List<Pedido>> PedidosPorEstadoAsync(int estadoId) =>
        _db.Pedidos
            .Where(p => p.EstadoPedidoId == estadoId)
            .ToListAsync();

    public Task<List<Pedido>> PedidosPorClienteAsync(int clienteId) =>
        _db.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();

    public Task<List<Pedido>> PedidosPorFechasAsync(
        DateTime inicio, DateTime fin) =>
        _db.Pedidos
            .Where(p => p.FechaPedido >= inicio &&
                        p.FechaPedido <= fin)
            .ToListAsync();

    // 4.3 Ordenamientos
    public Task<List<Cliente>> ClientesPorNombreAsync() =>
        _db.Clientes.OrderBy(c => c.Nombre).ToListAsync();

    public Task<List<Producto>> ProductosPorPrecioAsync() =>
        _db.Productos.OrderBy(p => p.Precio).ToListAsync();

    public Task<List<Producto>> ProductosPorStockAsync() =>
        _db.Productos.OrderByDescending(p => p.Stock).ToListAsync();

    public Task<List<Pedido>> PedidosPorFechaEntregaAsync() =>
        _db.Pedidos.OrderBy(p => p.FechaEntrega).ToListAsync();

    // 4.4 Agrupaciones
    public async Task<Dictionary<int, int>> PedidosAgrupadosPorEstadoAsync() =>
        await _db.Pedidos
            .GroupBy(p => p.EstadoPedidoId)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Cantidad);

    // 4.5 Joins
    public async Task<List<object>> ResumenPedidosAsync()
    {
        var datos = await (
            from pedido in _db.Pedidos
            join cliente in _db.Clientes
                on pedido.ClienteId equals cliente.Id
            join estado in _db.EstadosPedido
                on pedido.EstadoPedidoId equals estado.Id
            select new
            {
                PedidoId = pedido.Id,
                Cliente = cliente.Nombre,
                Estado = estado.Nombre,
                Fecha = pedido.FechaPedido,
                Entrega = pedido.FechaEntrega
            }
        ).ToListAsync();

        return datos.Cast<object>().ToList();
    }

    // 4.6 Consultas avanzadas
    public async Task<List<object>> ReporteProductosVendidosAsync()
    {
        var datos = await (
            from detalle in _db.DetallesPedido
            join producto in _db.Productos
                on detalle.ProductoId equals producto.Id
            group detalle by new { producto.Id, producto.Nombre } into g
            orderby g.Sum(d => d.Cantidad) descending
            select new
            {
                Producto = g.Key.Nombre,
                Unidades = g.Sum(d => d.Cantidad),
                Total = g.Sum(d => d.Cantidad * d.PrecioUnitario)
            }
        ).ToListAsync();

        return datos.Cast<object>().ToList();
    }

    // 4.6 Consulta avanzada: relacion entre cinco tablas
    public async Task<List<object>> DetalleCompletoPedidosAsync()
    {
        var datos = await (
            from pedido in _db.Pedidos
            join cliente in _db.Clientes
                on pedido.ClienteId equals cliente.Id
            join estado in _db.EstadosPedido
                on pedido.EstadoPedidoId equals estado.Id
            join detalle in _db.DetallesPedido
                on pedido.Id equals detalle.PedidoId
            join producto in _db.Productos
                on detalle.ProductoId equals producto.Id
            orderby pedido.FechaPedido descending
            select new
            {
                PedidoId = pedido.Id,
                Cliente = cliente.Nombre,
                Estado = estado.Nombre,
                Producto = producto.Nombre,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = detalle.Cantidad * detalle.PrecioUnitario,
                FechaPedido = pedido.FechaPedido
            }
        ).ToListAsync();

        return datos.Cast<object>().ToList();
    }
}

