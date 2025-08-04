using Microsoft.AspNetCore.Mvc;
using PhoneBook.Contracts.Dto;
using PhoneBook.DataModels;
using PhoneBook.Handlers;

namespace PhoneBook.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PhoneBookController(IConfiguration configuration, ContactsHandler contactsHandler, ExportHandler exportHandler) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    private readonly ContactsHandler _contactsHandler = contactsHandler ?? throw new ArgumentNullException(nameof(contactsHandler));

    private readonly ExportHandler _exportHandler = exportHandler ?? throw new ArgumentNullException(nameof(exportHandler));

    [HttpPost]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newContactDto = await _contactsHandler.CreateContactAsync(contactDto);
        return CreatedAtAction(nameof(GetContact), new { id = newContactDto.Id }, newContactDto);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateContact(ContactDto contactDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedContactDto = await _contactsHandler.UpdateContactAsync(contactDto);

        if (updatedContactDto is null)
        {
            return NotFound();
        }

        return Ok(updatedContactDto);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContact([FromQuery(Name = "id")] int id)
    {
        var deletionResult = await _contactsHandler.DeleteContactAsync(id);

        if (!deletionResult)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<ContactDto>> GetContact([FromQuery(Name = "id")] int id)
    {
        var contactDto = await _contactsHandler.GetContactAsync(id);

        if (contactDto is null)
        {
            return NotFound();
        }

        return contactDto;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContactDto>>> GetContacts(
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery(Name = "pageSize")] int pageSize = 20,
        [FromQuery(Name = "sortBy")] SortFieldType sortBy = SortFieldType.FirstName,
        [FromQuery(Name = "isSortDescending")] bool isSortDescending = false
    )
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("page и pageSize должны быть > 0");
        }

        return Ok(await _contactsHandler.GetContactsListAsync(page, pageSize, sortBy, isSortDescending));
    }

    [HttpGet]
    public async Task<IActionResult> ExportContactsToExcel()
    {
        return File(
            await _exportHandler.ExportContactsToExcelStreamAsync(),
            _configuration["ExcelContentType"]!,
            $"Contacts-{DateTime.Today:yyyy-MM-dd}.xlsx"
        );
    }
}
