using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.HRDirectorService;
using EveryoneToTheHackathon.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8083);
});

Int32.TryParse(builder.Configuration["EMPLOYEES_NUM"] ?? throw new SettingsException(), out var employeesNumber);
Int32.TryParse(builder.Configuration["HACKATHONS_NUM"] ?? throw new SettingsException(), out var hackathonsNumber);

var connString =
    String.Format(
        "Host={0};Port={1};Database={2};Username={3};Password={4};SSLMode=Prefer;Pooling=false",
        builder.Configuration["Database:Host"] ?? throw new SettingsException(),
        builder.Configuration["Database:Port"] ?? throw new SettingsException(),
        builder.Configuration["Database:Database"] ?? throw new SettingsException(),
        builder.Configuration["Database:Username"] ?? throw new SettingsException(),
        builder.Configuration["Database:Password"] ?? throw new SettingsException()
    );

builder.Services.AddDbContextFactory<AppDbContext>((provider, options) =>
{
    options.UseNpgsql(connString);
    options.EnableDetailedErrors();
    var context = new AppDbContext(options.Options);
    context.Database.Migrate();
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HrDirectorConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("amqp://rabbitmq:5672/"));
        cfg.ReceiveEndpoint($"HRDirector", e =>
            {
                e.ConfigureConsumers(context);
                e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            }
        );
    });
});

builder.Services.AddOptions();
builder.Services.Configure<ServiceSettings>(settings =>
{
    settings.EmployeesNumber = employeesNumber;
    settings.HackathonsNumber = hackathonsNumber;
});

builder.Services.AddSingleton<IHackathonRepository, HackathonRepository>();
builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddSingleton<IWishlistRepository, WishlistRepository>();
builder.Services.AddSingleton<ITeamRepository, TeamRepository>();

builder.Services.AddSingleton<HRDirector>();
builder.Services.AddSingleton<HrDirectorConsumer>();
builder.Services.AddSingleton<HrDirectorService>();

builder.Services.AddHostedService<HrDirectorBackgroundService>();

builder.Services.AddControllers().AddApplicationPart(typeof(HrDirectorController).Assembly);

var app = builder.Build();

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints => endpoints.MapControllers());
#pragma warning restore ASP0014


await app.RunAsync();