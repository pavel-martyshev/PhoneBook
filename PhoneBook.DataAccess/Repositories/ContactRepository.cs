using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataModels;

namespace PhoneBook.DataAccess.Repositories;

public class ContactRepository(PhoneBookContext db) : BaseRepository<Contact>(db), IContactRepository
{
    public List<ContactDto> GetContacts()
    {
        return _dbSet
            .AsNoTracking()
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                PhoneNumbers = c.PhoneNumbers
                    .Select(p => new PhoneNumberDto
                    {
                        Id = p.Id,
                        Number = p.Number,
                        Type = p.Type
                    })
                    .OrderBy(p => p.Number)
                    .ToList()
            })
            .OrderBy(c => c.LastName)
            .ToList();
    }
}
