
using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.ViewModels;

public class PedidoCrearViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
    public DateTime? FechaEntrega { get; set; }

    [StringLength(500,
        ErrorMessage = "Las observaciones no pueden superar 500 caracteres")]
    public string? Observaciones { get; set; }

    [Required(ErrorMessage = "Debe agregar productos al pedido")]
    [MinLength(1,
        ErrorMessage = "El pedido debe contener al menos un producto")]
    public List<DetallePedidoCrearViewModel> Detalles { get; set; } = new();
}
