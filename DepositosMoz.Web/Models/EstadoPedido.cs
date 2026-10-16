
using System.ComponentModel.DataAnnotations;

namespace DepositosMoz.Web.Models;

public class EstadoPedido
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del estado es obligatorio")]
    [StringLength(50)]
    public string Nombre { get; set; } = "";
}
