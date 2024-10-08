using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class EmployeeRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : IEmployeeRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddEmployee(Employee employee)
    {
        _dbContext.Add(employee);
        _dbContext.SaveChanges();
    }

    public IEnumerable<Employee> GetEmployeeByHackathonId(int hackathonId)
    {
        return _dbContext.Employees.Where(e => e.Hackathons.Select(h => h.Id).Contains(hackathonId)).ToList();
    }
}