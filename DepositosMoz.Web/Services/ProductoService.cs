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
        ArgumentNullException.ThrowIfNull(producto);

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            throw new ArgumentException(
                "El nombre del producto es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(producto.Descripcion))
        {
            throw new ArgumentException(
                "La descripción del producto es obligatoria");
        }

        if (producto.Precio <= 0)
        {
            throw new ArgumentException(
                "El precio debe ser mayor que cero");
        }

        if (producto.Stock < 0)
        {
            throw new ArgumentException(
                "El stock no puede ser negativo");
        }

        producto.Nombre = producto.Nombre.Trim();
        producto.Descripcion = producto.Descripcion.Trim();
        producto.Activo = true;

        await _repositorioProducto.AgregarAsync(producto);
    }

    public async Task<bool> ActualizarProductoAsync(Producto producto)
    {
        ArgumentNullException.ThrowIfNull(producto);

        var productoExistente =
            await _repositorioProducto.ObtenerPorIdAsync(producto.Id);

        if (productoExistente == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            throw new ArgumentException(
                "El nombre del producto es obligatorio");
        }

        if (string.IsNullOrWhiteSpace(producto.Descripcion))
        {
            throw new ArgumentException(
                "La descripción del producto es obligatoria");
        }

        if (producto.Precio <= 0)
        {
            throw new ArgumentException(
                "El precio debe ser mayor que cero");
        }

        if (producto.Stock < 0)
        {
            throw new ArgumentException(
                "El stock no puede ser negativo");
        }

        productoExistente.Nombre =
            producto.Nombre.Trim();

        productoExistente.Descripcion =
            producto.Descripcion.Trim();

        productoExistente.Precio =
            producto.Precio;

        productoExistente.Stock =
            producto.Stock;

        await _repositorioProducto.ActualizarAsync(
            productoExistente);

        return true;
    }

    public async Task<bool> DesactivarProductoAsync(int id)
    {
        var producto =
            await _repositorioProducto.ObtenerPorIdAsync(id);

        if (producto == null)
        {
            return false;
        }

        producto.Activo = false;

        await _repositorioProducto.ActualizarAsync(producto);

        return true;
    }
}
