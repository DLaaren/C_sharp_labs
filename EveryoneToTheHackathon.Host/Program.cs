using System.Text.Json;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Repositories;
using EveryoneToTheHackathon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", true, true);

Int32.TryParse(builder.Configuration["HackathonRounds"], out int totalRounds);
totalRounds = totalRounds == 0 ? 1000 : totalRounds;
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

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connString);
    options.EnableDetailedErrors();
});
//builder.Services.AddScoped<EmployeesSeedData>(_ => new EmployeesSeedData(teamLeads, juniors));

builder.Services.AddTransient<IHackathon, Hackathon>(
    h => new Hackathon(
        teamLeads,
        juniors,
        h.GetRequiredService<HRManager>(),
        h.GetRequiredService<HRDirector>()
    )
); 
builder.Services.AddTransient<ITeamBuildingStrategy, ProposeAndRejectAlgorithm>();
builder.Services.AddTransient<HRManager>();
builder.Services.AddTransient<HRDirector>();

builder.Services.AddTransient<IHackathonService, HackathonService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IWishlistService, WishlistService>();
builder.Services.AddTransient<ITeamService, TeamService>();

builder.Services.AddHostedService<DbHostedService>();

builder.Services.AddHostedService<HackathonHostedService>(h =>
    new HackathonHostedService(
        h,
        h.GetRequiredService<ILogger<HackathonHostedService>>(),
        totalRounds,
        h.GetRequiredService<IHackathonService>(),
        h.GetRequiredService<IEmployeeService>(),
        h.GetRequiredService<IWishlistService>(),
        h.GetRequiredService<ITeamService>()));

var host = builder.Build();

await host.RunAsync();