using DepositosMoz.Web.Models;
using DepositosMoz.Web.Repositories;

namespace DepositosMoz.Web.Services;

public class ProductoService : IProductoService
{
    private readonly IRepositorio<Producto> _repositorioProducto;

    public ProductoService(IRepositorio<Producto> repositorioProducto)
    {
        _repositorioProducto = repositorioProducto;
    }

    public async Task<List<Producto>> ObtenerProductosAsync()
    {
        return await _repositorioProducto.ObtenerTodosAsync();
    }

    public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
    {
        return await _repositorioProducto.ObtenerPorIdAsync(id);
    }

    public async Task CrearProductoAsync(Producto producto)
    {
        await _repositorioProducto.AgregarAsync(producto);
    }
}
