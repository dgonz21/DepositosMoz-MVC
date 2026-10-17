using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Web.Controllers;

[Route("Productos")]
public class ProductosController : Controller
{
    private readonly IDatosService _datosService;

    public ProductosController(IDatosService datosService)
    {
        _datosService = datosService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var productos = _datosService.ObtenerProductos();

        return View(productos);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(ProductoCrearViewModel modelo)
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
            _datosService.AgregarProducto(producto);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }
}