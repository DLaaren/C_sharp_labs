using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.EmployeeService;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

var teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads5.csv", EmployeeTitle.TeamLead);
var juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors5.csv", EmployeeTitle.Junior);
var hrManagerUri = new Uri(builder.Configuration["HrManagerUri"] ?? throw new SettingsException());

var id = builder.Configuration["ID"] ?? throw new NullReferenceException();
var title = builder.Configuration["TITLE"] ?? throw new NullReferenceException();
var name = builder.Configuration["NAME"] ?? throw new NullReferenceException();
var employee = new Employee(Convert.ToInt32(id), title, name);

builder.Services.AddHttpClient<EmployeeService>(client => client.BaseAddress = hrManagerUri);
builder.Services.AddHostedService<EmployeeService>(e => 
    new EmployeeService(
        e.GetRequiredService<ILogger<EmployeeService>>(),
        e.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(EmployeeService)),
        employee,
        employee.Title.Equals("TeamLead") ? juniors : teamLeads));

var app = builder.Build();
await app.RunAsync();