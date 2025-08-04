namespace PhoneBook.Contracts.Repositories;

public interface IUnitOfWork : IUnitOfWorkBase
{
    IContactRepository ContactRepository { get; }
}
