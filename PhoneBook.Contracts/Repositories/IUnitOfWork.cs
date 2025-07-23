namespace PhoneBook.Contracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    public IContactRepository ContactRepository { get; }

    public Task SaveAsync();

    public Task BeginTransactionAsync();

    public Task RollbackTransactionAsync();
}
