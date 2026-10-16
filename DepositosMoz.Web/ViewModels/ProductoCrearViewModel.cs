
using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.ViewModels;

public class ProductoCrearViewModel
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    [StringLength(250)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0.01", "999999.99",
        ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
}
