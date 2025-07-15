using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess;
using PhoneBook.DataModels;

namespace PhoneBook;

public class DbInitializer
{
    private readonly PhoneBookContext _db;

    public DbInitializer(PhoneBookContext dbContext) => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public void Initialize()
    {
        _db.Database.Migrate();

        if (!_db.Contacts.Any())
        {
            _db.Contacts.Add(new Contact
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

            _db.Contacts.Add(new Contact
            {
                FirstName = "Петр",
                MiddleName = "Петрович",
                LastName = "Петров",
                PhoneNumbers =
                [
                    new PhoneNumber()
                    {
                        Number = "+79995558877",
                        Type = PhoneNumberType.Mobile
                    }
                ]
            });

            _db.Contacts.Add(new Contact
            {
                FirstName = "Петр",
                MiddleName = "Петрович",
                LastName = "Петров",
                PhoneNumbers =
                [
                    new PhoneNumber()
                                {
                                    Number = "+10987654321",
                                    Type = PhoneNumberType.Work
                                }
                ]
            });

            _db.SaveChanges();
        }
    }
}
