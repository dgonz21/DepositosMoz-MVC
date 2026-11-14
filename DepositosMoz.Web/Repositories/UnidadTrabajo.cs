using DepositosMoz.Web.Data;

namespace DepositosMoz.Web.Repositories;

public class UnidadTrabajo : IUnidadTrabajo
{
    private readonly DepositosMozDbContext _contexto;

    public UnidadTrabajo(DepositosMozDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _contexto.SaveChangesAsync();
    }

    public async Task EjecutarEnTransaccionAsync(
        Func<Task> operacion)
    {
        ArgumentNullException.ThrowIfNull(operacion);

        await using var transaccion =
            await _contexto.Database.BeginTransactionAsync();

        try
        {
            await operacion();

            await transaccion.CommitAsync();
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }
}
