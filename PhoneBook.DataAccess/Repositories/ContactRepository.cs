using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataModels;
using System.Linq.Dynamic.Core;

namespace PhoneBook.DataAccess.Repositories;

public class ContactRepository(PhoneBookContext db) : BaseRepository<Contact>(db), IContactRepository
{
    public async Task<ContactDto?> GetContactAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted && c.Id == id)
            .Select(c => new ContactDto()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                MiddleName = c.MiddleName,
                LastName = c.LastName,
                PhoneNumbers = c.PhoneNumbers
                .Where(p => !p.IsDeleted)
                .Select(p => new PhoneNumberDto()
                {
                    Number = p.Number,
                    Type = p.Type
                })
                .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<ContactDto>> GetAllContactsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                MiddleName = c.MiddleName,
                PhoneNumbers = c.PhoneNumbers
                    .Where(p => !p.IsDeleted)
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
            .ToListAsync();
    }

    public async Task<List<ContactDto>> GetContactsAsync(int page, int pageSize, SortFieldType sortBy, bool isSortDescending)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy($"{sortBy} {(isSortDescending ? "DESC" : "ASC")}")
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ContactDto()
            {
                Id = c.Id,
                FirstName = c.FirstName,
                MiddleName = c.MiddleName,
                LastName = c.LastName,
                PhoneNumbers = c.PhoneNumbers
                    .Where(p => !p.IsDeleted)
                    .Select(p => new PhoneNumberDto()
                    {
                        Id = p.Id,
                        Number = p.Number,
                        Type = p.Type
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
