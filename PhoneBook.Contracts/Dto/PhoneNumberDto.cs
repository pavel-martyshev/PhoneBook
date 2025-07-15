using PhoneBook.DataModels;

namespace PhoneBook.Contracts.Dto;

public class PhoneNumberDto
{
    public int Id { get; set; }

    public required string Number { get; set; }

    public PhoneNumberType Type { get; set; }
}
