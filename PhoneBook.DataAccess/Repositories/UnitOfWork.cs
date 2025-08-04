using PhoneBook.Contracts.Repositories;

namespace PhoneBook.DataAccess.Repositories;

public class UnitOfWork(PhoneBookContext db) : UnitOfWorkBase(db), IUnitOfWork
{
    public IContactRepository ContactRepository { get; } = new ContactRepository(db);
}
