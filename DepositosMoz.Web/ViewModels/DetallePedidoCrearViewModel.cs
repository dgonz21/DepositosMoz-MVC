
using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.ViewModels;

public class DetallePedidoCrearViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un producto")]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; } = 1;
}
