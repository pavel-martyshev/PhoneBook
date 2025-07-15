using PhoneBook.Contracts.Dto;
using PhoneBook.DataModels;

namespace PhoneBook.Contracts.Repositories;

public interface IContactRepository : IRepository<Contact>
{
    public List<ContactDto> GetContacts();
}
