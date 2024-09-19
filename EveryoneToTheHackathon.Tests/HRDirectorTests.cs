using System.Reflection;
using EveryoneToTheHackathon.DataContracts;
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
            new Wishlist(1,  Enumerable.Range(1, 5).OrderBy(_ => new Random(1).Next()).ToArray()),
            new Wishlist(2, Enumerable.Range(1, 5).OrderBy(_ => new Random(2).Next()).ToArray()),
            new Wishlist(3, Enumerable.Range(1, 5).OrderBy(_ => new Random(3).Next()).ToArray()),
            new Wishlist(4, Enumerable.Range(1, 5).OrderBy(_ => new Random(4).Next()).ToArray()),
            new Wishlist(5, Enumerable.Range(1, 5).OrderBy(_ => new Random(5).Next()).ToArray())
        };
        List<Wishlist> juniorsWishlists = new List<Wishlist>
        {
            new Wishlist(1,  Enumerable.Range(1, 5).OrderBy(_ => new Random(10).Next()).ToArray()),
            new Wishlist(2, Enumerable.Range(1, 5).OrderBy(_ => new Random(20).Next()).ToArray()),
            new Wishlist(3, Enumerable.Range(1, 5).OrderBy(_ => new Random(30).Next()).ToArray()),
            new Wishlist(4, Enumerable.Range(1, 5).OrderBy(_ => new Random(40).Next()).ToArray()),
            new Wishlist(5, Enumerable.Range(1, 5).OrderBy(_ => new Random(50).Next()).ToArray())
        };
        List<Team> teams = new List<Team>
        {
            new Team(new Employee(1, "John Doe"), new Employee(4, "Jane Jordan")),
            new Team(new Employee(5, "Chuck Norris"), new Employee(3, "Jack Jones")),
            new Team(new Employee(2, "Jane Black"), new Employee(1, "Walter White")),
            new Team(new Employee(3, "Bob Richman"), new Employee(2, "Arnold Kindman")),
            new Team(new Employee(4, "Aboba Abobovich"), new Employee(5, "Ken Kennedy"))
        };
        HRDirector hrDirector = new HRDirector();
        
        // Act
        double satisfactionIndex = hrDirector.CalculateMeanSatisfactionIndex(teamLeadsWishlists, juniorsWishlists, teams);
        
        // Assert
        Assert.Equal(3, satisfactionIndex);
    }
}