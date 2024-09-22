using System.Text.Json;
using EveryoneToTheHackathon;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", true, true);

List<Employee> teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads20.csv", EmployeeTitle.TeamLead);
List<Employee> juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
    builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors20.csv", EmployeeTitle.Junior);

string connString =
    String.Format(
        "Host={0};Port={1};Database={2};Username={3};Password={4};SSLMode=Prefer;Pooling=false",
        builder.Configuration["Database:Host"] ?? throw new JsonException(),
        builder.Configuration["Database:Port"] ?? throw new JsonException(),
        builder.Configuration["Database:Database"] ?? throw new JsonException(),
        builder.Configuration["Database:Username"] ?? throw new JsonException(),
        builder.Configuration["Database:Password"] ?? throw new JsonException()
    );

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connString));
builder.Services.AddScoped<EmployeesSeedData>(_ => new EmployeesSeedData(teamLeads, juniors));

builder.Services.AddTransient<IHackathon, Hackathon>(
    h => new Hackathon(
        CsvParser.ParseCsvFileWithEmployees(builder.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads20.csv", EmployeeTitle.TeamLead),
        20,
        CsvParser.ParseCsvFileWithEmployees(builder.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors20.csv", EmployeeTitle.Junior),
        20,
        h.GetRequiredService<HRManager>(),
        h.GetRequiredService<HRDirector>()
    )
); 
builder.Services.AddScoped<ITeamBuildingStrategy, ProposeAndRejectAlgorithm>();
builder.Services.AddScoped<HRManager>();
builder.Services.AddScoped<HRDirector>();

builder.Services.AddHostedService<DbHostedService>();

builder.Services.AddHostedService<HackathonHostedService>(h => 
    new HackathonHostedService(
        h.GetRequiredService<ILogger<HackathonHostedService>>(), 
        h.GetRequiredService<IHackathon>(), 
        1));

var host = builder.Build();
await host.RunAsync();