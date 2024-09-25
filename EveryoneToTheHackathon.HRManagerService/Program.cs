using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.HRManagerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8082);
});

var employeesNumber = builder.Configuration.GetValue<int>("EMPLOYEES_NUM");
Uri hrDirectorUrl = new Uri("http://hr_director:8083");

builder.Services.AddMvc();
builder.Services.AddTransient<HRManager>(_ =>
    new HRManager(new ProposeAndRejectAlgorithm()));

builder.Services.AddOptions();
builder.Services.Configure<ControllerSettings>(settings => settings.EmployeesNumber = employeesNumber);

builder.Services.AddHostedService<HRManagerService>(e =>
    new HRManagerService(
        e.GetRequiredService<ILogger<HRManagerService>>(),
        e.GetRequiredService<HttpClient>(),
        e.GetRequiredService<HRManager>()));
        //employeesNumber));

//builder.Services.AddMvc().AddControllersAsServices().AddApplicationPart(typeof(HRManagerController).Assembly);
builder.Services.AddControllers().AddApplicationPart(typeof(HRManagerController).Assembly);
builder.Services.AddHttpClient<HRManagerService>(options => options.BaseAddress = hrDirectorUrl);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

await app.RunAsync();