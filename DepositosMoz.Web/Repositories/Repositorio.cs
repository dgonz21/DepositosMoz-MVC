using System.Linq.Expressions;
using DepositosMoz.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DepositosMoz.Web.Repositories;

public class Repositorio<T> : IRepositorio<T> where T : class
{
    private readonly DepositosMozDbContext _contexto;
    private readonly DbSet<T> _tabla;

    public Repositorio(DepositosMozDbContext contexto)
    {
        _contexto = contexto;
        _tabla = contexto.Set<T>();
    }

    public async Task<List<T>> ObtenerTodosAsync()
    {
        return await _tabla.AsNoTracking().ToListAsync();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _tabla.FindAsync(id);
    }

    public async Task<List<T>> FiltrarAsync(Expression<Func<T, bool>> filtro)
    {
        return await _tabla.AsNoTracking()
            .Where(filtro)
            .ToListAsync();
    }

    public async Task AgregarAsync(T entidad)
    {
        await _tabla.AddAsync(entidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarAsync(T entidad)
    {
        _tabla.Update(entidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task EliminarAsync(T entidad)
    {
        _tabla.Remove(entidad);
        await _contexto.SaveChangesAsync();
    }
}
