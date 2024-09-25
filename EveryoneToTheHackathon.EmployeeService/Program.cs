using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.EmployeeService;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

List<Employee> teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads5.csv", EmployeeTitle.TeamLead);
List<Employee> juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors5.csv", EmployeeTitle.Junior);
Uri hrManagerUrl = new Uri("http://hr_manager:8082");

var id = builder.Configuration["ID"] ?? throw new NullReferenceException();
var title = builder.Configuration["TITLE"] ?? throw new NullReferenceException();
var name = builder.Configuration["NAME"] ?? throw new NullReferenceException();
Employee employee = new Employee(Convert.ToInt32(id), title, name);

builder.Services.AddControllers();
builder.Services.AddHostedService<EmployeeService>(e => 
    new EmployeeService(
        e.GetRequiredService<ILogger<EmployeeService>>(),
        e.GetRequiredService<HttpClient>(),
        hrManagerUrl,
        employee,
        employee.Title.Equals("TeamLead") ? juniors : teamLeads));
builder.Services.AddHttpClient<EmployeeService>();

var app = builder.Build();
await app.RunAsync();