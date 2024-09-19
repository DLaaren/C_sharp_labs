using System.Text.Json;
using EveryoneToTheHackathon.DataContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EveryoneToTheHackathon.Host;

public static class Program
{
    static void Main(string[] args)
    {
        HostApplicationBuilderSettings settings = new()
        {
            Args = args,
            ApplicationName = "Nsu.HackathonProblem",
            Configuration = new ConfigurationManager(),
            ContentRootPath = Directory.GetCurrentDirectory(),
        };
        settings.Configuration.AddJsonFile("appsettings.json", true, true);
        settings.Configuration.AddCommandLine(args);
        
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(settings);
        
        string connString =
            String.Format(
                "Host={0};Port={1};Database={2};Username={3};Password={4};SSLMode=Prefer;Pooling=false",
                settings.Configuration["Database:Host"] ?? throw new JsonException(),
                settings.Configuration["Database:Port"] ?? throw new JsonException(),
                settings.Configuration["Database:Database"] ?? throw new JsonException(),
                settings.Configuration["Database:Username"] ?? throw new JsonException(),
                settings.Configuration["Database:Password"] ?? throw new JsonException()
                );
        
        ApplicationContext applicationContext = new ApplicationContext(connString);
        
        /*
         * The order of registration has no affect
         */
        builder.Services.AddHostedService<HackathonService>(
            h => new HackathonService(
                h.GetRequiredService<ILogger<HackathonService>>(),
                h.GetRequiredService<IHackathon>(),
                1000
                )
            );

        List<Employee> teamLeads = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
            settings.Configuration["Resources:TeamLeadsList"] ?? "Resources/Teamleads20.csv", EmployeeTitle.TeamLead);
        List<Employee> juniors = (List<Employee>)CsvParser.ParseCsvFileWithEmployees(
            settings.Configuration["Resources:JuniorsList"] ?? "Resources/Juniors20.csv", EmployeeTitle.Junior);

        applicationContext.Employees.AddRange(teamLeads);
        applicationContext.Employees.AddRange(juniors);
        
        builder.Services.AddTransient<IHackathon, Hackathon>(
            h => new Hackathon(
                teamLeads,
                20,
                juniors,
                20,
                h.GetRequiredService<HRManager>(),
                h.GetRequiredService<HRDirector>()
                )
            ); 
        builder.Services.AddTransient<ITeamBuildingStrategy, ProposeAndRejectAlgorithm>();
        builder.Services.AddTransient<HRManager>();
        builder.Services.AddTransient<HRDirector>();
        
        /*
         * AddTransient = сервис создаётся каждый раз, когда его запрашивают. Подходит для легковесных, не фиксирующих состояние сервисов
         * AddScoped = сервис создаётся единожды для каждого запроса
         * AddSingleton = сервис создаётся при первом запросе, а при последующих используется тот же инстанс
         */
        
        IHost host = builder.Build();
        host.Run();
        applicationContext.Database.CloseConnection();
    }
}
