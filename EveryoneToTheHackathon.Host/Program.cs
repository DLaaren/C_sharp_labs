using EveryoneToTheHackathon.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;

//TODO in another project

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
        builder.Services.AddTransient<IHackathon, Hackathon>(
            h => new Hackathon(
                CsvParser.ParseCsvFileWithEmployees(settings.Configuration["TeamLeadsList"] ?? "Resources/Teamleads20.csv"),
                20,
                CsvParser.ParseCsvFileWithEmployees(settings.Configuration["JuniorsList"] ?? "Resources/Juniors20.csv"),
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
    }
}
