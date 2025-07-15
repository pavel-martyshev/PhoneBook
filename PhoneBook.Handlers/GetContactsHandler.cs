using ClosedXML.Excel;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.Handlers;

public class GetContactsHandler(IContactRepository contactRepository)
{
    private readonly IContactRepository _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));

    public List<ContactDto> GetContactsList()
    {
        return _contactRepository.GetContacts();
    }

    public void ExportContactsToExcel(string filePath)
    {
        var contacts = _contactRepository.GetContacts()
            .Select(c => new
            {
                c.Id,
                c.FirstName,
                c.MiddleName,
                c.LastName,
                HomeNumbers = string.Join(", ", c.PhoneNumbers
                    .Where(p => p.Type == DataModels.PhoneNumberType.Home)
                    .Select(p => p.Number)),
                MobileNumbers = string.Join(", ", c.PhoneNumbers
                    .Where(p => p.Type == DataModels.PhoneNumberType.Mobile)
                    .Select(p => p.Number)),
                WorkNumbers = string.Join(", ", c.PhoneNumbers
                    .Where(p => p.Type == DataModels.PhoneNumberType.Work)
                    .Select(p => p.Number))
            });

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Contacts");

        ws.Cell("A1").InsertTable(contacts, tableName: "Contacts");

        var header = ws.Range("A1:D1");
        header.Style.Font.Bold = true;
        header.Style.Fill.PatternType = XLFillPatternValues.Solid;
        header.Style.Fill.SetBackgroundColor(XLColor.BlueGray);

        foreach (var columnAddress in ws.Cells())
        {
            columnAddress.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            columnAddress.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        ws.Columns().AdjustToContents();

        wb.SaveAs(filePath);
    }
}
