using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.EmployeeService;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081);
});

var teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads5.csv", EmployeeTitle.TeamLead);
var juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors5.csv", EmployeeTitle.Junior);
var hrManagerUri = new Uri(builder.Configuration["HrManagerUri"] ?? throw new SettingsException());

var id = builder.Configuration["ID"] ?? "1";//throw new NullReferenceException();
var title = builder.Configuration["TITLE"] ?? "Junior";//throw new NullReferenceException();
var name = builder.Configuration["NAME"] ?? "Puos";//throw new NullReferenceException();
var employee = new Employee(Convert.ToInt32(id), title, name);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeBackgroundService>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("amqp://rabbitmq:5672/"));
        cfg.ReceiveEndpoint($"Employee-{employee.Id}-{employee.Title}", e => 
        {
            e.ConfigureConsumers(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        });
    });
});

builder.Services.AddHttpClient<EmployeeBackgroundService>(client => client.BaseAddress = hrManagerUri);
builder.Services.AddMvc();
builder.Services.AddOptions();
builder.Services.Configure<ServiceSettings>(settings =>
{
    settings.Employee = employee;
    settings.ProbableTeammates = employee.Title.Equals("TeamLead") ? juniors : teamLeads;
});
builder.Services.AddSingleton<EmployeeService>(e => new EmployeeService(e.GetService<IOptions<ServiceSettings>>()!));
builder.Services.AddHostedService<EmployeeBackgroundService>(e => 
    new EmployeeBackgroundService(
        e.GetRequiredService<IBusControl>(),
        e.GetRequiredService<ILogger<EmployeeBackgroundService>>(),
        e.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(EmployeeBackgroundService)),
        e.GetRequiredService<EmployeeService>()));

builder.Services.AddControllers().AddApplicationPart(typeof(EmployeeController).Assembly);

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();