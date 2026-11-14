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

    [HttpGet("Editar/{id:int}")]
    public async Task<IActionResult> Editar(int id)
    {
        var cliente =
            await _clienteService.ObtenerClientePorIdAsync(id);

        if (cliente == null)
        {
            return NotFound();
        }

        var modelo = new ClienteEditarViewModel
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Telefono = cliente.Telefono,
            Direccion = cliente.Direccion
        };

        return View(modelo);
    }

    [HttpPost("Editar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        ClienteEditarViewModel modelo)
    {
        if (id != modelo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var cliente = new Cliente
        {
            Id = modelo.Id,
            Nombre = modelo.Nombre,
            Telefono = modelo.Telefono,
            Direccion = modelo.Direccion
        };

        try
        {
            var actualizado =
                await _clienteService.ActualizarClienteAsync(cliente);

            if (!actualizado)
            {
                return NotFound();
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Desactivar/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var desactivado =
            await _clienteService.DesactivarClienteAsync(id);

        if (!desactivado)
        {
            return NotFound();
        }

        TempData["Mensaje"] =
            "El cliente fue desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
