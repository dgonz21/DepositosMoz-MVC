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
        public async Task<IActionResult> Crear(ProductoCrearViewModel modelo)   
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
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }
}
