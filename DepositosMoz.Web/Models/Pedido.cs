
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DepositosMoz.Web.Models;

public class Pedido
{
    public int Id { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un cliente")]
    public int ClienteId { get; set; }

    public DateTime FechaPedido { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
    public DateTime FechaEntrega { get; set; }

    public int EstadoPedidoId { get; set; } = 1;

    [StringLength(500)]
    public string? Observaciones { get; set; }

    public List<DetallePedido> Detalles { get; set; } = new();

    public decimal Total => Detalles.Sum(detalle => detalle.Subtotal);
}
