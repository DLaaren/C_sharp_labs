using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Tests;

public class DatabaseFixture : IDisposable
{
    public readonly List<Employee> TeamLeads;
    public readonly List<Employee> Juniors;
    public readonly List<Wishlist> TeamLeadsWishlists;
    public readonly List<Wishlist> JuniorsWishlists;
    public readonly List<Team> Teams;

    public DatabaseFixture()
    {
        TeamLeads =
        [
            new Employee(1, EmployeeTitle.TeamLead, "John Doe"),
            new Employee(2, EmployeeTitle.TeamLead, "Jane Black"),
            new Employee(3, EmployeeTitle.TeamLead, "Bob Richman"),
            new Employee(4, EmployeeTitle.TeamLead, "Aboba Abobovich"),
            new Employee(5, EmployeeTitle.TeamLead, "Chuck Norris")
        ];
        Juniors =
        [
            new Employee(1, EmployeeTitle.Junior, "Walter White"),
            new Employee(2, EmployeeTitle.Junior, "Arnold Kindman"),
            new Employee(3, EmployeeTitle.Junior, "Jack Jones"),
            new Employee(4, EmployeeTitle.Junior, "Jane Jordan"),
            new Employee(5, EmployeeTitle.Junior, "Ken Kennedy")
        ];

        TeamLeadsWishlists = new List<Wishlist>(5);
        JuniorsWishlists = new List<Wishlist>(5);
        for (var i = 1; i <= TeamLeads.Count; i++)
        {
            var seed = new Random(i);
            TeamLeadsWishlists.Add(new Wishlist(i, EmployeeTitle.TeamLead, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }
        for (var i = 1; i <= Juniors.Count; i++)
        {
            var seed = new Random(i * 100);
            JuniorsWishlists.Add(new Wishlist(i, EmployeeTitle.Junior, Enumerable.Range(1, 5).OrderBy(_ => seed.Next()).ToArray()));
        }
        
        Teams =
        [
            new Team(new Employee(1, EmployeeTitle.TeamLead, "John Doe"),
                new Employee(2, EmployeeTitle.Junior, "Arnold Kindman")),
            new Team(new Employee(3, EmployeeTitle.TeamLead, "Bob Richman"),
                new Employee(4, EmployeeTitle.Junior, "Jane Jordan")),
            new Team(new Employee(4, EmployeeTitle.TeamLead, "Aboba Abobovich"),
                new Employee(5, EmployeeTitle.Junior, "Ken Kennedy")),
            new Team(new Employee(5, EmployeeTitle.TeamLead, "Chuck Norris"),
                new Employee(3, EmployeeTitle.Junior, "Jack Jones")),
            new Team(new Employee(2, EmployeeTitle.TeamLead, "Jane Black"),
                new Employee(1, EmployeeTitle.Junior, "Walter White"))
        ];
    }

    public void Dispose() { }
}