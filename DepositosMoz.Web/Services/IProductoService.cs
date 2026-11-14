using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IProductoService
{
    Task<List<Producto>> ObtenerProductosAsync();

    Task<Producto?> ObtenerProductoPorIdAsync(int id);

    Task CrearProductoAsync(Producto producto);

    Task<bool> ActualizarProductoAsync(Producto producto);

    Task<bool> DesactivarProductoAsync(int id);
}
