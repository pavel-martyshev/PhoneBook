namespace PhoneBook.DataModels;

public class BaseModel
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public bool IsDeleted { get; set; }
}
