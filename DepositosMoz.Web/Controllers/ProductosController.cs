using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Web.Controllers;

[Route("Productos")]
public class ProductosController : Controller
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.ObtenerProductosAsync();

        return View(productos);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        ProductoCrearViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var producto = new Producto
        {
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion,
            Precio = modelo.Precio,
            Stock = modelo.Stock
        };

        try
        {
            await _productoService.CrearProductoAsync(producto);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var producto =
            await _productoService.ObtenerProductoPorIdAsync(id);

        if (producto == null)
        {
            return NotFound();
        }

        var modelo = new ProductoEditarViewModel
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            Stock = producto.Stock
        };

        return View(modelo);
    }

    [HttpPost("Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        ProductoEditarViewModel modelo)
    {
        if (id != modelo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var producto = new Producto
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion,
            Precio = modelo.Precio,
            Stock = modelo.Stock
        };

        try
        {
            var actualizado =
                await _productoService.ActualizarProductoAsync(
                    producto);

            if (!actualizado)
            {
                return NotFound();
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Desactivar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var desactivado =
            await _productoService.DesactivarProductoAsync(id);

        if (!desactivado)
        {
            return NotFound();
        }

        TempData["Mensaje"] =
            "El producto fue desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
