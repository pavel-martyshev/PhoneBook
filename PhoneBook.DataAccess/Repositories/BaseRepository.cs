using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.DataAccess.Repositories;

public class BaseRepository<T>(DbContext db) : IRepository<T> where T : class
{
    protected DbContext _db = db;

    protected DbSet<T> _dbSet = db.Set<T>();

    public void Create(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        _db.Attach(entity);
        _db.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _db.Attach(entity);
        }

        _dbSet.Remove(entity);
    }

    public T[] GetAll()
    {
        return _dbSet.ToArray();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }
}
