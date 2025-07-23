using PhoneBook.Contracts.Dto;
using PhoneBook.DataModels;

namespace PhoneBook.Contracts.Repositories;

public interface IContactRepository : IRepository<Contact>
{
    public Task<ContactDto?> GetContactAsync(int id);

    public Task<List<ContactDto>> GetAllContactsAsync();

    public Task<List<ContactDto>> GetContactsAsync(int page, int pageSize, SortFieldType sortBy, bool isSortDescending);
}
