
using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.Models;

public class DetallePedido
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar un producto")]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; }

    [Range(typeof(decimal), "0.01", "999999.99",
        ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}
