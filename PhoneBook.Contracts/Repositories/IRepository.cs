namespace PhoneBook.Contracts.Repositories;

public interface IRepository<T> where T : class
{
    public Task CreateAsync(T entity);

    public void Update(T entity);

    public void Delete(T entity);

    public Task<T[]> GetAllAsync();

    public ValueTask<T?> GetByIdAsync(int id);
}
