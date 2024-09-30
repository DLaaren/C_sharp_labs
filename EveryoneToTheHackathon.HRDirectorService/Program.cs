using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.HRDirectorService;
using EveryoneToTheHackathon.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8083);
});

var employeesNumber = builder.Configuration.GetValue<int>("EMPLOYEES_NUM");

var connString =
    String.Format(
        "Host={0};Port={1};Database={2};Username={3};Password={4};SSLMode=Prefer;Pooling=false",
        builder.Configuration["Database:Host"] ?? throw new SettingsException(),
        builder.Configuration["Database:Port"] ?? throw new SettingsException(),
        builder.Configuration["Database:Database"] ?? throw new SettingsException(),
        builder.Configuration["Database:Username"] ?? throw new SettingsException(),
        builder.Configuration["Database:Password"] ?? throw new SettingsException()
    );

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseNpgsql(connString);
    options.EnableDetailedErrors();
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("amqp://rabbitmq:5672/"));
    });
});

builder.Services.AddMvc();
builder.Services.AddSingleton<HRDirector>();
builder.Services.AddSingleton<IHackathonRepository, HackathonRepository>();

builder.Services.AddOptions();
builder.Services.Configure<ControllerSettings>(settings => settings.EmployeesNumber = employeesNumber);
builder.Services.AddSingleton<HrDirectorService>();

builder.Services.AddHostedService<HrDirectorBackgroundService>();

builder.Services.AddControllers().AddApplicationPart(typeof(HrDirectorController).Assembly);

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();