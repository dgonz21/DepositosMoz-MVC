using DepositosMoz.Web.Models;

namespace DepositosMoz.Web.Services;

public interface IProductoService
{
    Task<List<Producto>> ObtenerProductosAsync();

    Task<Producto?> ObtenerProductoPorIdAsync(int id);

    Task CrearProductoAsync(Producto producto);
}
