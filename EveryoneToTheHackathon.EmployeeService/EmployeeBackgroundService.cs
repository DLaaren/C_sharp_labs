using System.Text;
using System.Text.Json;
using EveryoneToTheHackathon.Dtos;
using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Messages;
using MassTransit;

namespace EveryoneToTheHackathon.EmployeeService;

public class EmployeeBackgroundService(
    IBusControl busControl,
    ILogger<EmployeeBackgroundService> logger,
    HttpClient httpClient,
    EmployeeService employeeService)
    : BackgroundService, IConsumer<HackathonStarted>
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            employeeService.HackathonStartedTcs = new TaskCompletionSource<bool>();
            
            logger.LogInformation("Waiting for a hackathon to start");
            await employeeService.HackathonStartedTcs.Task;
            
            employeeService.ResetAll();

            //await SendEmployeeAsync(employeeService.Employee, stoppingToken);
            await SendEmployeeAsyncViaMessage(employeeService.Employee, stoppingToken);
            logger.LogInformation("Employee 'Id = {Id} Title = {Title} Name = {Name}' has sent his data;",
                employeeService.Employee.Id, employeeService.Employee.Title, employeeService.Employee.Name);

            logger.LogInformation("Employee 'Id = {Id} Title = {Title} Name = {Name}' is making his wishlist;",
                employeeService.Employee.Id, employeeService.Employee.Title, employeeService.Employee.Name);
            var wishlist = employeeService.Employee.MakeWishlist(employeeService.ProbableTeammates);

            //await SendWishlistAsync(wishlist, stoppingToken);
            await SendWishlistAsyncViaMessage(wishlist, stoppingToken);
            logger.LogInformation(
                "Employee 'Id = {Id} Title = {Title} Name = {Name}' has sent his wishlist: {Wishlist};",
                employeeService.Employee.Id, employeeService.Employee.Title, employeeService.Employee.Name,
                wishlist.DesiredEmployees);
        }

        await Task.CompletedTask;
    }

    private async Task SendEmployeeAsync(Employee employeeToSend, CancellationToken stoppingToken)
    {
        var employeeDto = new EmployeeDto(employeeToSend.Id, employeeToSend.Title, employeeToSend.Name);
        var content = new StringContent(JsonSerializer.Serialize(employeeDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/employee", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task SendEmployeeAsyncViaMessage(Employee employeeToSend, CancellationToken stoppingToken)
    {
        await busControl.Publish<EmployeeSent>(new {
            employeeToSend.Id, 
            employeeToSend.Title,
            employeeToSend.Name
        }, stoppingToken);
    }
    
    private async Task SendWishlistAsyncViaMessage(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        await busControl.Publish<WishlistSent>(new  WishlistSent(
            wishlistToSend.EmployeeId, 
            wishlistToSend.EmployeeTitle,
            wishlistToSend.DesiredEmployees), stoppingToken);
    }
    
    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(wishlistToSend.EmployeeId, wishlistToSend.EmployeeTitle, wishlistToSend.DesiredEmployees);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/wishlist", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }

    public Task Consume(ConsumeContext<HackathonStarted> context)
    {
        logger.LogInformation(context.Message.Message);
        employeeService.HackathonStartedTcs.TrySetResult(true);
        return Task.CompletedTask;
    }
}