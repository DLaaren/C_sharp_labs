using System.Reflection;
using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Tests;

public class HRDirectorTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void CheckCalculationOfMean()
    {
        // Arrange
        var type = typeof(HRDirector);
        var director = Activator.CreateInstance(type);
        var privateMethod = type.GetMethod("CalculateMean", BindingFlags.NonPublic | BindingFlags.Instance);
        const int count = 10;
        var numbers = Enumerable.Range(1, count).ToArray();
        object[] parameters1 = [numbers, count];
        var sameNumbers = Enumerable.Repeat(count, count).ToArray();
        object[] parameters2 = [sameNumbers, count];
        
        // Act
        var res1 = (double?)privateMethod?.Invoke(director, parameters1);
        var res2 = (double?)privateMethod?.Invoke(director, parameters2);
        
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
        HRDirector hrDirector = new HRDirector();
        
        // Act
        double satisfactionIndex = hrDirector.CalculateMeanSatisfactionIndex(fixture.TeamLeadsWishlists, fixture.JuniorsWishlists, fixture.Teams);
        
        // Assert
        Assert.Equal(4.2, satisfactionIndex);
    }
}