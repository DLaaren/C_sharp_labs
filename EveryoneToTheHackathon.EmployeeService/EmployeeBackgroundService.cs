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
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Waiting for a hackathon to start");
        while (true) {}
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task SendEmployeeAsync(Employee employeeToSend, CancellationToken stoppingToken)
    {
        var employeeDto = new EmployeeDto(employeeToSend.Id, employeeToSend.Title, employeeToSend.Name);
        var content = new StringContent(JsonSerializer.Serialize(employeeDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/employee", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
    
    private async Task SendWishlistAsync(Wishlist wishlistToSend, CancellationToken stoppingToken)
    {
        var wishlistDto = new WishlistDto(wishlistToSend.EmployeeId, wishlistToSend.EmployeeTitle, wishlistToSend.DesiredEmployees);
        var content = new StringContent(JsonSerializer.Serialize(wishlistDto), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(httpClient.BaseAddress + "api/hr_manager/wishlist", content, stoppingToken);
        response.EnsureSuccessStatusCode();
    }
    
    private void SendEmployeeAndWishlistStoredAsyncViaMessage()
    {
        var employee = employeeService.Employee;
        busControl.Publish(
            new EmployeeAndWishlistSent($"Employee 'Id = {employee.Id} " +
                                        $"Title = {employee.Title} " +
                                        $"Name = {employee.Name}' " +
                                        $"has stored his data", 
                employee.Id, employee.Title, employee.Name));
        logger.LogInformation("Employee has send his hackathon confirmation");
    }

    public Task Consume(ConsumeContext<HackathonStarted> context)
    {
        var hackathonId = context.Message.HackathonId;
        
        logger.LogInformation("Hackathon with id = {hackathonId} has started", hackathonId);
        
        var wishlist = employeeService.Employee.MakeWishlist(employeeService.ProbableTeammates);
        
        employeeService.SaveEmployeeAndWishlist(wishlist, hackathonId);
        logger.LogInformation("Employee has stored his data");
            
        SendEmployeeAndWishlistStoredAsyncViaMessage();
        
        logger.LogInformation("Waiting for a hackathon to start");
        return Task.CompletedTask;
    }
}