using EveryoneToTheHackathon.Entities;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Repositories;

public class EmployeeRepository(IDbContextFactory<AppDbContext> myDbContextFactory) : IEmployeeRepository
{
    private readonly AppDbContext _dbContext = myDbContextFactory.CreateDbContext();
    
    public void AddEmployee(Employee employee)
    {
        _dbContext.Entry(employee).State = EntityState.Added;
        _dbContext.Entry(employee.Wishlists.Last()).State = EntityState.Added;
    }

    public void UpdateEmployee(Employee employee)
    {
        if (!_dbContext.Employees.Any(e => e.Id == employee.Id && e.Title == employee.Title))
            AddEmployee(employee);
        else
            _dbContext.Update(employee);
}

    public void SaveEmployee(Employee employee, Hackathon hackathon)
    {
        _dbContext.Entry(hackathon).State = EntityState.Modified;
        UpdateEmployee(employee);
        _dbContext.SaveChanges();
    }

    public IEnumerable<Employee> GetEmployeeByHackathonId(int hackathonId)
    {
        return _dbContext.Employees.Where(e => e.Hackathons.Select(h => h.Id).Contains(hackathonId)).ToList();
    }

    public IEnumerable<Employee> GetEmployeesByTeamId(int teamId)
    {
        return _dbContext.Employees.Where(e => e.Teams.Select(t => t.Id).Contains(teamId)).ToList();
    }
}