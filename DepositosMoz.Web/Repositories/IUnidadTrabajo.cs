namespace DepositosMoz.Web.Repositories;

public interface IUnidadTrabajo
{
    Task<int> GuardarCambiosAsync();

    Task EjecutarEnTransaccionAsync(
        Func<Task> operacion);
}
