using DepositosMoz.Web.DTOs;
using DepositosMoz.Web.Models;
using DepositosMoz.Web.Services;
using DepositosMoz.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DepositosMoz.Web.Controllers;

[Route("Pedidos")]
public class PedidosController : Controller
{
    private readonly IDatosService _datosService;

    public PedidosController(IDatosService datosService)
    {
        _datosService = datosService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var pedidos = _datosService
            .ObtenerPedidos()
            .Select(pedido => new PedidoResumenDto
            {
                Id = pedido.Id,

                NombreCliente =
                    _datosService
                        .ObtenerClientePorId(pedido.ClienteId)?
                        .Nombre
                    ?? "Cliente no encontrado",

                FechaEntrega = pedido.FechaEntrega,

                Estado = ObtenerNombreEstado(
                    pedido.EstadoPedidoId),

                Total = pedido.Total
            })
            .ToList();

        return View(pedidos);
    }

    [HttpGet("Crear")]
    public IActionResult Crear()
    {
        CargarDatosFormulario();

        var modelo = new PedidoCrearViewModel
        {
            Detalles = new List<DetallePedidoCrearViewModel>
            {
                new DetallePedidoCrearViewModel()
            }
        };

        return View(modelo);
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(PedidoCrearViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            CargarDatosFormulario();

            return View(modelo);
        }

        var pedido = new Pedido
        {
            ClienteId = modelo.ClienteId,
            FechaEntrega = modelo.FechaEntrega!.Value,
            Observaciones = modelo.Observaciones,

            Detalles = modelo.Detalles
                .Select(detalle => new DetallePedido
                {
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad
                })
                .ToList()
        };

        try
        {
            _datosService.AgregarPedido(pedido);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            CargarDatosFormulario();

            return View(modelo);
        }

        return RedirectToAction(
            nameof(Detalle),
            new { id = pedido.Id });
    }

    [HttpGet("Detalle/{id:int}")]
    public IActionResult Detalle(int id)
    {
        var pedido =
            _datosService.ObtenerPedidoPorId(id);

        if (pedido == null)
        {
            return NotFound();
        }

        var cliente =
            _datosService.ObtenerClientePorId(
                pedido.ClienteId);

        ViewBag.Cliente = cliente;

        ViewBag.Estado =
            ObtenerNombreEstado(
                pedido.EstadoPedidoId);

        ViewBag.Estados =
            ObtenerEstados();

        ViewBag.Productos =
            _datosService
                .ObtenerProductos()
                .ToDictionary(
                    producto => producto.Id,
                    producto => producto.Nombre);

        return View(pedido);
    }

    [HttpPost("CambiarEstado/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult CambiarEstado(
        int id,
        int estadoId)
    {
        var pedido =
            _datosService.ObtenerPedidoPorId(id);

        if (pedido == null)
        {
            return NotFound();
        }

        if (estadoId < 1 || estadoId > 4)
        {
            return BadRequest(
                "El estado seleccionado no es válido.");
        }

        var actualizado =
            _datosService.CambiarEstadoPedido(
                id,
                estadoId);

        if (!actualizado)
        {
            return BadRequest(
                "No fue posible actualizar el estado del pedido.");
        }

        TempData["Mensaje"] =
            "El estado del pedido fue actualizado correctamente.";

        return RedirectToAction(
            nameof(Detalle),
            new { id });
    }

    private void CargarDatosFormulario()
    {
        var clientes =
            _datosService
                .ObtenerClientes()
                .Where(cliente => cliente.Activo)
                .ToList();

        var productos =
            _datosService
                .ObtenerProductos()
                .Where(producto => producto.Activo)
                .ToList();

        ViewBag.Clientes =
            new SelectList(
                clientes,
                "Id",
                "Nombre");

        ViewBag.Productos =
            new SelectList(
                productos,
                "Id",
                "Nombre");
    }

    private static string ObtenerNombreEstado(
        int estadoId)
    {
        return estadoId switch
        {
            1 => "Pendiente",
            2 => "En preparación",
            3 => "Listo para entrega",
            4 => "Entregado",
            _ => "Desconocido"
        };
    }

    private static List<EstadoPedido> ObtenerEstados()
    {
        return new List<EstadoPedido>
        {
            new EstadoPedido
            {
                Id = 1,
                Nombre = "Pendiente"
            },

            new EstadoPedido
            {
                Id = 2,
                Nombre = "En preparación"
            },

            new EstadoPedido
            {
                Id = 3,
                Nombre = "Listo para entrega"
            },

            new EstadoPedido
            {
                Id = 4,
                Nombre = "Entregado"
            }
        };
    }
}