using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DepositosMoz.Web.Controllers;

[Route("Clientes")]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var clientes = await _clienteService.ObtenerClientesAsync();

        return View(clientes);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ClienteCrearViewModel modelo)    
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
            await _clienteService.CrearClienteAsync(cliente);        
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }
}
