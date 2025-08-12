using Microsoft.Data.SqlClient;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataModels;
using System.Text.RegularExpressions;

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

        try
        {
            await _uow.BeginTransactionAsync();
            await _uow.ContactRepository.CreateAsync(contact);
            await _uow.SaveAsync();
        }
        catch (Exception e)
        {
            if (e.InnerException is SqlException sqlError && sqlError.Number == 2601)
            {
                var match = Regex.Match(e.InnerException.Message, @":\s*\(+(.*?)\)");

                if (match.Success)
                {
                    var duplicatedPhoneNumber = match
                        .ToString()
                        .Replace(": (", "")
                        .TrimEnd(')');

                    throw new ArgumentException($"Номер {duplicatedPhoneNumber} уже существует.");
                }
            }

            throw;
        }
        finally
        {
            await _uow.RollbackTransactionAsync();
        }

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
            await _uow.RollbackTransactionAsync();
            return null;
        }

        contact.FirstName = contactDto.FirstName;
        contact.MiddleName = contactDto.MiddleName;
        contact.LastName = contactDto.LastName;

        foreach (var phoneNumber in contact.PhoneNumbers)
        {
            if (!contactDto.PhoneNumbers.Any(p => p.Number == phoneNumber.Number))
            {
                phoneNumber.IsDeleted = true;
            }
        }

        foreach (var dtoPhoneNumber in contactDto.PhoneNumbers)
        {
            var phoneNumber = contact.PhoneNumbers
                .Where(p => p.Number == dtoPhoneNumber.Number)
                .FirstOrDefault();

            if (phoneNumber is null)
            {
                var newPhoneNumber = new PhoneNumber
                {
                    Number = dtoPhoneNumber.Number,
                    Type = dtoPhoneNumber.Type
                };

                contact.PhoneNumbers.Add(newPhoneNumber);
            }
            else
            {
                phoneNumber.Type = dtoPhoneNumber.Type;
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
