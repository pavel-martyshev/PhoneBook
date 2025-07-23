using ClosedXML.Excel;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.Handlers;

public class ExportHandler(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    private async Task<T> ExportContacts<T>(Func<XLWorkbook, T> saver)
    {
        var contactsList = await _unitOfWork.ContactRepository.GetAllContactsAsync();

        var contacts = contactsList
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

        return await Task.Run(() =>
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Contacts");

            ws.Cell("A1").InsertTable(contacts, tableName: "Contacts");

            var header = ws.Range("A1:G1");
            header.Style.Font.Bold = true;
            header.Style.Fill.PatternType = XLFillPatternValues.Solid;
            header.Style.Fill.SetBackgroundColor(XLColor.BlueGray);

            var usedCellsRange = ws.RangeUsed();
            usedCellsRange!.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            usedCellsRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();

            return saver(wb);
        });
    }

    public Task ExportContactsToExcelOnDisk(string filePath)
    {
        return ExportContacts(wb =>
        {
            wb.SaveAs(filePath);
            return true;
        });
    }

    public Task<byte[]> ExportContactsToExcelStream()
    {
        return ExportContacts(wb =>
        {
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        });
    }
}
