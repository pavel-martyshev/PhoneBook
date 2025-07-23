using PhoneBook.Handlers;
using Quartz;

namespace PhoneBook.Jobs;

public class ExportContactsToExcelJob(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, ExportHandler exportHandler) : IJob
{
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    private readonly IConfiguration _configuration = configuration;

    private readonly ExportHandler _exportHandler = exportHandler ?? throw new ArgumentNullException(nameof(exportHandler));

    public Task Execute(IJobExecutionContext context)
    {
        return _exportHandler.ExportContactsToExcelOnDisk(Path.Combine(
            _webHostEnvironment.ContentRootPath,
            _configuration["UnloadingDirectory"] ?? "",
            $"Contacts-{DateTime.Now:yyyy-MM-dd HH-mm}.xlsx"
        ));
    }
}
