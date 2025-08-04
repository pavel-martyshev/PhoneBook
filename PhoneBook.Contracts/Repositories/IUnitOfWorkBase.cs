namespace PhoneBook.Contracts.Repositories;

public interface IUnitOfWorkBase : IDisposable
{
    public Task SaveAsync();

    public Task BeginTransactionAsync();

    public Task RollbackTransactionAsync();
}
