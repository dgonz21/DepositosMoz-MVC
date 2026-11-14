using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.ViewModels;

public class ProductoEditarViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La descripción del producto es obligatoria")]
    [StringLength(200)]
    public string Descripcion { get; set; } = "";

    [Range(typeof(decimal), "0.01", "999999.99",
        ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }
}
