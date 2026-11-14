using System.Linq.Expressions;

namespace DepositosMoz.Web.Repositories;

public interface IRepositorio<T> where T : class
{
    Task<List<T>> ObtenerTodosAsync();
    Task<T?> ObtenerPorIdAsync(int id);
    Task<List<T>> FiltrarAsync(Expression<Func<T, bool>> filtro);
    Task AgregarAsync(T entidad);
    Task ActualizarAsync(T entidad);
    Task EliminarAsync(T entidad);
}
