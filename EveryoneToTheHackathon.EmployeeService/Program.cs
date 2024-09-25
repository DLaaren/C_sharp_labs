using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

List<Employee> teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads5.csv", EmployeeTitle.TeamLead);
List<Employee> juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors5.csv", EmployeeTitle.Junior);
Uri hrManagerUrl = new Uri(builder.Configuration["Resources:HRManagerUrl"] ?? "http://localhost:5001");

string id = Environment.GetEnvironmentVariable("id", EnvironmentVariableTarget.User) ?? throw new NullReferenceException();
string title = Environment.GetEnvironmentVariable("title", EnvironmentVariableTarget.User) ?? throw new NullReferenceException();
string name = Environment.GetEnvironmentVariable("name", EnvironmentVariableTarget.User) ?? throw new NullReferenceException();
Employee employee = new Employee(Convert.ToInt32(id), title, name);

builder.Services.AddControllers();
builder.Services.AddHttpClient<EmployeeService>();

builder.Services.AddHostedService<EmployeeService>(e => 
    new EmployeeService(
        e.GetRequiredService<ILogger<EmployeeService>>(),
        e.GetRequiredService<HttpClient>(),
        hrManagerUrl,
        employee,
        employee.Title.Equals("TeamLead") ? juniors : teamLeads));

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

//app.UseHttpsRedirection();

//app.MapControllers(); 

await app.RunAsync();