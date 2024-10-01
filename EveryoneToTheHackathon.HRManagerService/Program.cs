using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.HRManagerService;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082);
});

var employeesNumber = builder.Configuration.GetValue<int>("EMPLOYEES_NUM");
var hrDirectorUrl = new Uri(builder.Configuration["HrDirectorUri"] ?? throw new SettingsException());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<HrManagerBackgroundService>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("amqp://rabbitmq:5672/"));
        cfg.ReceiveEndpoint($"HRManager", e => 
        {
            e.ConfigureConsumers(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        });
    });
});

builder.Services.AddHttpClient<HrManagerBackgroundService>(options => options.BaseAddress = hrDirectorUrl);
builder.Services.AddMvc();
builder.Services.AddSingleton<HRManager>(_ => new HRManager(new ProposeAndRejectAlgorithm()));

builder.Services.AddOptions();
builder.Services.Configure<ControllerSettings>(settings => settings.EmployeesNumber = employeesNumber);
builder.Services.AddSingleton<HrManagerService>();

builder.Services.AddHostedService<HrManagerBackgroundService>(s => 
    new HrManagerBackgroundService(
        s.GetRequiredService<ILogger<HrManagerBackgroundService>>(),
        s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(HrManagerBackgroundService)),
        s.GetRequiredService<HrManagerService>()));

builder.Services.AddControllers().AddApplicationPart(typeof(HrManagerController).Assembly);

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();