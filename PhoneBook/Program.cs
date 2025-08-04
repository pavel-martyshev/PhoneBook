using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess;
using PhoneBook.DataAccess.Repositories;
using PhoneBook.Handlers;
using PhoneBook.Jobs;
using Quartz;

namespace PhoneBook;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<DbInitializer>();
        builder.Services.AddDbContext<PhoneBookContext>(options =>
        {
            options
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Transient);

        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<ContactsHandler>();
        builder.Services.AddTransient<ExportHandler>();

        builder.Services.AddQuartz(q =>
            q.ScheduleJob<ExportContactsToExcelJob>(trigger => trigger
                .WithIdentity(nameof(ExportContactsToExcelJob))
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever()
        )));

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        //app.UseAuthorization();
        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var dbInitializer = services.GetRequiredService<DbInitializer>();
                await dbInitializer.Initialize();
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "An error occurred while seeding the database.");
                throw;
            }
        }

        app.Run();
    }
}
