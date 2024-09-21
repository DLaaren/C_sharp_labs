using System.Reflection;
using EveryoneToTheHackathon.Entities;
using Xunit.Abstractions;

namespace EveryoneToTheHackathon.Tests;

public class HRDirectorTests
{
    [Fact]
    public void CheckCalculationOfMean()
    {
        // Arrange
        Type type = typeof(HRDirector);
        var director = Activator.CreateInstance(type);
        var privateMethod = type.GetMethod("CalculateMean", BindingFlags.NonPublic | BindingFlags.Instance);
        int count = 10;
        int[] numbers = Enumerable.Range(1, count).ToArray();
        object[] parameters1 = { numbers, count };
        int[] sameNumbers = Enumerable.Repeat(count, count).ToArray();
        object[] parameters2 = { sameNumbers, count };
        
        // Act
        double? res1 = (double?)privateMethod?.Invoke(director, parameters1);
        double? res2 = (double?)privateMethod?.Invoke(director, parameters2);
        
        // Assert
        Assert.NotNull(res1);
        Assert.Equal(res1, 5.5);
        Assert.NotNull(res2);
        Assert.Equal(res2, count);
    }
    
    [Fact]
    public void CheckCalculationOfMeanSatisfactionIndexWithDefinedData()
    {
        // Arrange
        List<Wishlist> teamLeadsWishlists = new List<Wishlist>
        {
            new Wishlist(1, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => new Random(1).Next()).ToArray()),
            new Wishlist(2, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => new Random(2).Next()).ToArray()),
            new Wishlist(3, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => new Random(3).Next()).ToArray()),
            new Wishlist(4, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => new Random(4).Next()).ToArray()),
            new Wishlist(5, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => new Random(5).Next()).ToArray())
        };
        List<Wishlist> juniorsWishlists = new List<Wishlist>
        {
            new Wishlist(1, EmployeeTitle.Junior,  Enumerable.Range(1, 5).OrderBy(_ => new Random(10).Next()).ToArray()),
            new Wishlist(2, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => new Random(20).Next()).ToArray()),
            new Wishlist(3, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => new Random(30).Next()).ToArray()),
            new Wishlist(4, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => new Random(40).Next()).ToArray()),
            new Wishlist(5, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => new Random(50).Next()).ToArray())
        };
        List<Team> teams = new List<Team>
        {
            new Team(new Employee(1, EmployeeTitle.TeamLead,"John Doe"), new Employee(4, EmployeeTitle.Junior,"Jane Jordan")),
            new Team(new Employee(5, EmployeeTitle.TeamLead,"Chuck Norris"), new Employee(3, EmployeeTitle.Junior,"Jack Jones")),
            new Team(new Employee(2, EmployeeTitle.TeamLead,"Jane Black"), new Employee(1, EmployeeTitle.Junior,"Walter White")),
            new Team(new Employee(3, EmployeeTitle.TeamLead,"Bob Richman"), new Employee(2, EmployeeTitle.Junior,"Arnold Kindman")),
            new Team(new Employee(4, EmployeeTitle.TeamLead,"Aboba Abobovich"), new Employee(5, EmployeeTitle.Junior,"Ken Kennedy"))
        };
        HRDirector hrDirector = new HRDirector();
        
        // Act
        double satisfactionIndex = hrDirector.CalculateMeanSatisfactionIndex(teamLeadsWishlists, juniorsWishlists, teams);
        
        // Assert
        Assert.Equal(3, satisfactionIndex);
    }
}