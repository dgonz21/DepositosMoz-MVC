
namespace DepositosMoz.Web.DTOs;

public class PedidoResumenDto
{
    public int Id { get; set; }

    public string NombreCliente { get; set; } = "";

    public DateTime FechaEntrega { get; set; }

    public string Estado { get; set; } = "";

    public decimal Total { get; set; }
}
