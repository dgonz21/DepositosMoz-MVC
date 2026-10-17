using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Web.Controllers;

[Route("Clientes")]
public class ClientesController : Controller
{
    private readonly IDatosService _datosService;

    public ClientesController(IDatosService datosService)
    {
        _datosService = datosService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var clientes = _datosService.ObtenerClientes();

        return View(clientes);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(ClienteCrearViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var cliente = new Cliente
        {
            Nombre = modelo.Nombre,
            Telefono = modelo.Telefono,
            Direccion = modelo.Direccion
        };

        try
        {
            _datosService.AgregarCliente(cliente);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }
}