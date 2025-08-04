using Microsoft.EntityFrameworkCore;
using PhoneBook.DataModels;

namespace PhoneBook.DataAccess;

public class DbInitializer(PhoneBookContext dbContext)
{
    private readonly PhoneBookContext _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task Initialize()
    {
        _db.Database.Migrate();

        Console.WriteLine(await _db.Contacts.AnyAsync());

        if (await _db.Contacts.AnyAsync())
        {
            return;
        }

        await _db.Contacts.AddAsync(new Contact
        {
            FirstName = "Иван",
            MiddleName = "Иванович",
            LastName = "Иванов",
            PhoneNumbers =
            [
                new PhoneNumber()
                {
                    Number = "+12345678901",
                    Type = PhoneNumberType.Mobile
                }
            ]
        });

        await _db.Contacts.AddAsync(new Contact
        {
            FirstName = "Петр",
            MiddleName = "Петрович",
            LastName = "Петров",
            PhoneNumbers =
            [
                new PhoneNumber
                {
                    Number = "+79995558877",
                    Type = PhoneNumberType.Mobile
                },
                new PhoneNumber
                {
                    Number = "+10987654321",
                    Type = PhoneNumberType.Work
                }
            ]
        });

        await _db.SaveChangesAsync();
    }
}
