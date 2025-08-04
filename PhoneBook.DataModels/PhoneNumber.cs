namespace PhoneBook.DataModels;

public class PhoneNumber : BaseModel
{
    public string Number { get; set; } = null!;

    public PhoneNumberType Type { get; set; }

    public int ContactId { get; set; }

    public virtual Contact Contact { get; set; } = null!;
}
