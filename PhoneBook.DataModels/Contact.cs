namespace PhoneBook.DataModels;

public class Contact : BaseModel
{
    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual List<PhoneNumber> PhoneNumbers { get; set; } = [];
}
