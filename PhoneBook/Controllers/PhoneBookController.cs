using Microsoft.AspNetCore.Mvc;
using PhoneBook.Contracts.Dto;
using PhoneBook.Handlers;

namespace PhoneBook.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PhoneBookController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, GetContactsHandler getContactsHandler) : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly IConfiguration _configuration = configuration;

    private readonly GetContactsHandler _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));

    public List<ContactDto> GetContacts()
    {
        return _getContactsHandler.GetContactsList();
    }

    public IActionResult ExportContactsToExcel()
    {
        var filename = $"Contacts-{DateTime.Today:yyyy-MM-dd}.xlsx";
        var filePath = Path.Combine(
            _webHostEnvironment.ContentRootPath,
            _configuration["TempFilesDirectory"] ?? "",
            filename
        );

        _getContactsHandler.ExportContactsToExcel(filePath);

        return PhysicalFile(filePath, "application/xlsx", filename);
    }
}
