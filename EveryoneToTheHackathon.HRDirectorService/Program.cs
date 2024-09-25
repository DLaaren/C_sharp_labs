using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using EveryoneToTheHackathon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

var employeesNumber = builder.Configuration["EMPLOYEES_NUM"] ?? throw new NullReferenceException();
HRDirector hrDirector = new HRDirector();

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

builder.Services.AddControllers();
builder.Services.AddSingleton<IHackathonRepository, HackathonRepository>();
builder.Services.AddHostedService<HRDirectorService>(e => 
    new HRDirectorService(
        e.GetRequiredService<ILogger<HRDirectorService>>(),
        e.GetRequiredService<HttpClient>(),
        hrDirector,
        Convert.ToInt32(employeesNumber),
        e.GetRequiredService<IHackathonRepository>()));
builder.Services.AddHttpClient<HRDirectorService>();

var app = builder.Build();
await app.RunAsync();