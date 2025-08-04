using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataModels;

namespace PhoneBook.Handlers;

public class ContactsHandler(IUnitOfWork uow)
{
    private readonly IUnitOfWork _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    public Task<ContactDto?> GetContactAsync(int id)
    {
        return _uow.ContactRepository.GetContactAsync(id);
    }

    public Task<List<ContactDto>> GetContactsListAsync(int page, int pageSize, SortFieldType sortBy, bool isSortDescending)
    {
        return _uow.ContactRepository.GetContactsAsync(page, pageSize, sortBy, isSortDescending);
    }

    public async Task<bool> DeleteContactAsync(int id)
    {
        await _uow.BeginTransactionAsync();
        var contact = await _uow.ContactRepository.GetByIdAsync(id);

        if (contact is null)
        {
            return false;
        }

        _uow.ContactRepository.Delete(contact);
        await _uow.SaveAsync();

        return true;
    }

    public async Task<ContactDto> CreateContactAsync(ContactDto contactDto)
    {
        var contact = new Contact
        {
            FirstName = contactDto.FirstName,
            MiddleName = contactDto.MiddleName,
            LastName = contactDto.LastName,
            PhoneNumbers = contactDto.PhoneNumbers
                .Select(p => new PhoneNumber
                {
                    Number = p.Number,
                    Type = p.Type
                })
                .ToList()
        };

        await _uow.BeginTransactionAsync();
        await _uow.ContactRepository.CreateAsync(contact);
        await _uow.SaveAsync();

        return new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            MiddleName = contact.MiddleName,
            LastName = contact.LastName,
            PhoneNumbers = contact.PhoneNumbers
                .Select(p => new PhoneNumberDto
                {
                    Id = p.Id,
                    Number = p.Number,
                    Type = p.Type
                })
                .ToList()
        };
    }

    public async Task<ContactDto?> UpdateContactAsync(ContactDto contactDto)
    {
        await _uow.BeginTransactionAsync();
        var contact = await _uow.ContactRepository.GetByIdAsync(contactDto.Id);

        if (contact is null)
        {
            return null;
        }

        contact.FirstName = contactDto.FirstName;
        contact.MiddleName = contactDto.MiddleName;
        contact.LastName = contactDto.LastName;

        foreach (var number in contact.PhoneNumbers)
        {
            if (!contactDto.PhoneNumbers.Any(p => p.Number == number.Number))
            {
                number.IsDeleted = true;
            }
        }

        foreach (var dtoNumber in contactDto.PhoneNumbers)
        {
            if (!contact.PhoneNumbers.Any(p => p.Number == dtoNumber.Number))
            {
                var number = new PhoneNumber()
                {
                    Number = dtoNumber.Number,
                    Type = dtoNumber.Type
                };

                contact.PhoneNumbers.Add(number);
            }

            if (contact.PhoneNumbers.Any(p => p.Number == dtoNumber.Number && p.Type != dtoNumber.Type))
            {
                var number = contact.PhoneNumbers
                    .Where(p => p.Number == dtoNumber.Number)
                    .FirstOrDefault();

                number!.Type = dtoNumber.Type;
            }
        }

        _uow.ContactRepository.Update(contact);
        await _uow.SaveAsync();

        return new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            MiddleName = contact.MiddleName,
            LastName = contact.LastName,
            PhoneNumbers = contact.PhoneNumbers
                .Select(p => new PhoneNumberDto
                {
                    Id = p.Id,
                    Number = p.Number,
                    Type = p.Type
                })
                .ToList()
        };
    }
}
