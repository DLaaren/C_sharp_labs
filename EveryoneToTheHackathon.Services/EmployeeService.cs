using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Services;

public class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    private readonly AppDbContext _dbContext = dbContext;

    public Employee? GetEmployeeById(int employeeId)
    {
        return _dbContext.Employees.Find(employeeId);
    }
    
    public IEnumerable<Employee> GetEmployees()
    {
        return _dbContext.Employees.ToList();
    }

    public void AddEmployee(Employee employee)
    {
        _dbContext.Add(employee);
        _dbContext.SaveChanges();
    }
    
    public void AddEmployees(IEnumerable<Employee> employees)
    {
        _dbContext.AddRange(employees);
        _dbContext.SaveChanges();
    }
    
    public void UpdateEmployee(Employee employee)
    {
        _dbContext.Update(employee);
        _dbContext.SaveChanges();
    }
    
    public void UpdateEmployees(IEnumerable<Employee> employees)
    {
        _dbContext.UpdateRange(employees);
        _dbContext.SaveChanges();
    }
}