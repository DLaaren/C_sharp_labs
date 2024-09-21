using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Host;
using EveryoneToTheHackathon.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services.Services;

public class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        return await _dbContext.Employees.ToListAsync();
    }
    
    public async Task AddEmployeesAsync(IEnumerable<Employee> employees)
    {
        _dbContext.AddRange(employees);
        await _dbContext.SaveChangesAsync();
    }
}