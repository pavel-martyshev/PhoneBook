using PhoneBook.Handlers;
using Quartz;

namespace PhoneBook.Jobs;

public class ExportContactsToExcelJob(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, GetContactsHandler getContactsHandler) : IJob
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly IConfiguration _configuration = configuration;

    private GetContactsHandler _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));

    public Task Execute(IJobExecutionContext context)
    {

        _getContactsHandler.ExportContactsToExcel(Path.Combine(
            _webHostEnvironment.ContentRootPath,
            _configuration["UnloadingDirectory"] ?? "",
            $"Contacts-{DateTime.Now:yyyy-MM-dd HH-mm}.xlsx"
        ));

        return Task.CompletedTask;
    }
}
