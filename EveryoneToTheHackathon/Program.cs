using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EveryoneToTheHackathon;

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
        
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(settings);
        // hosted services start sequencely not concurrently (but it can be configured as concurrent services)
        builder.Services.AddHostedService<HackathonService>();
        builder.Services.AddTransient<Hackathon>(); // using lambda for constructing (can be done some additional work; let us control the creation) let us to clearly create new instance of the class for each request
        builder.Services.AddTransient<ITeamBuildingStrategy, ProposeAndRejectAlgorithm>(); // registering interface with such realization; every time an interface is requested DI container notices it and creates new instance of this interface
        //builder.Services.AddTransient<ITeamBuildingStrategy, None>();
        builder.Services.AddTransient<HRManager>(); // simply creates an instance
        builder.Services.AddTransient<HRDirector>();
        /*
         * The order of registration has no affect
         */
        
        /*
         * AddTransient = сервис создаётся каждый раз, когда его запрашивают. Подходит для легковесных, не фиксирующих состояние сервисов
         * AddScoped = сервис создаётся единожды для каждого запроса
         * AddSingleton = сервис создаётся при первом запросе, а при последующих используется тот же инстанс
         */
        
        /*
         * Host controls lifecycle of the app and gives infrastructure for DI aka DI container
         * Dependency = service
         */
        IHost host = builder.Build();
        host.Run();
    }
}
