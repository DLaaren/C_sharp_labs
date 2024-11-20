using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Nsu.HackathonProblem.Contracts;

namespace Nsu.HackathonProblem.Vinter.Strategy;

public class ProposeAndRejectAlgorithm : ITeamBuildingStrategy
{
    private static List<Team> PerformAlgorithmWithPriorityToGroup(List<Employee> group1, List<Employee> group2, 
        List<Wishlist> group1Wishlists, List<Wishlist> group2Wishlists, string priorityToGroup)
    {
        group1Wishlists = group1Wishlists.ToList();
        group2Wishlists = group2Wishlists.ToList();
        var teams = new List<Team>();
        
        var freeFromGroup1 = new bool[group1.Count + 1];
        var freeFromGroup2 = new bool[group2.Count + 1];
        for (var i = 1; i < freeFromGroup1.Length; i++)
            freeFromGroup1[i] = true;
        for (var i = 1; i < freeFromGroup2.Length; i++)
            freeFromGroup2[i] = true;
        
        while (freeFromGroup1.Contains(true))
        {
            var freePerson1 = group1.Find(t => t.Id == Array.FindIndex(freeFromGroup1, free => free));
            Debug.Assert(freePerson1 != null, nameof(freePerson1) + " == null");
            
            var freeFromGroup1Wishlist = group1Wishlists.Find(w => w.EmployeeId == freePerson1.Id);
            Debug.Assert(freeFromGroup1Wishlist != null, nameof(freeFromGroup1Wishlist) + " == null");
            
            var mostWantedPerson2Id = freeFromGroup1Wishlist.DesiredEmployees.First();
            
            var mostWantedPerson2 = group2.Find(e => e.Id == mostWantedPerson2Id);
            Debug.Assert(mostWantedPerson2 != null, nameof(mostWantedPerson2) + " == null");
            
            if (freeFromGroup2[mostWantedPerson2Id])
            {   
                switch (priorityToGroup)
                {
                    case "TeamLeads":
                        teams.Add(new Team(freePerson1, mostWantedPerson2));
                        break;
                    
                    case "Juniors":
                        teams.Add(new Team(mostWantedPerson2, freePerson1));
                        break;
                        
                    default: 
                        Debug.Fail($"Unknown priority to group {priorityToGroup}");
                        break;
                }
                
                freeFromGroup1[freePerson1.Id] = false;
                freeFromGroup2[mostWantedPerson2Id] = false;
                
                continue;
            }

            Employee? currentPerson1 = null; 
            
            switch (priorityToGroup)
            {
                case "TeamLeads":
                    currentPerson1 = teams.Find(t => t.Junior.Id == mostWantedPerson2Id)?.TeamLead;
                    break;
                    
                case "Juniors":
                    currentPerson1 = teams.Find(t => t.TeamLead.Id == mostWantedPerson2Id)?.Junior;
                    break;
                        
                default: 
                    Debug.Fail($"Unknown priority to group {priorityToGroup}");
                    break;
            }
            Debug.Assert(currentPerson1 != null, nameof(currentPerson1) + " == null");
            
            var wishlistOfMostWantedPerson2 = group2Wishlists.Find(w => w.EmployeeId == mostWantedPerson2Id);
            Debug.Assert(wishlistOfMostWantedPerson2 != null, nameof(wishlistOfMostWantedPerson2) + " == null");
            
            var freePerson1Priority = 20 - Array.IndexOf(wishlistOfMostWantedPerson2.DesiredEmployees, freePerson1.Id);
            var currentPerson1Priority = 20 - Array.IndexOf(wishlistOfMostWantedPerson2.DesiredEmployees, currentPerson1.Id);
            
            if (freePerson1Priority > currentPerson1Priority)
            {
                
                Team? removedTeam = null;
                switch (priorityToGroup)
                {
                    case "TeamLeads":
                        removedTeam = teams.Find(t => t.Junior.Id == mostWantedPerson2Id);
                        break;
                    
                    case "Juniors":
                        removedTeam = teams.Find(t => t.TeamLead.Id == mostWantedPerson2Id);
                        break;
                        
                    default: 
                        Debug.Fail($"Unknown priority to group {priorityToGroup}");
                        break;
                }
                Debug.Assert(removedTeam != null, nameof(removedTeam) + " == null");
                teams.Remove(removedTeam);
                
                switch (priorityToGroup)
                {
                    case "TeamLeads":
                        teams.Add(new Team(freePerson1, mostWantedPerson2));
                        break;
                    
                    case "Juniors":
                        teams.Add(new Team(mostWantedPerson2, freePerson1));
                        break;
                        
                    default: 
                        Debug.Fail($"Unknown priority to group {priorityToGroup}");
                        break;
                }
                
                freeFromGroup1[freePerson1.Id] = false;
                freeFromGroup2[mostWantedPerson2Id] = false;

                var currentPerson1WishlistIds = group1Wishlists.Find(w => w.EmployeeId == currentPerson1.Id)?.DesiredEmployees;
                Debug.Assert(currentPerson1WishlistIds != null, nameof(currentPerson1WishlistIds) + " == null");
                
                var newCurrentPerson1WishlistIds = currentPerson1WishlistIds.Where(id => id != Array.IndexOf(currentPerson1WishlistIds, mostWantedPerson2.Id)).ToArray();
                var newCurrentPerson1Wishlist = new Wishlist(currentPerson1.Id, newCurrentPerson1WishlistIds);
                group1Wishlists[group1Wishlists.FindIndex(w => w.EmployeeId == currentPerson1.Id)] =
                    newCurrentPerson1Wishlist;
                
                freeFromGroup1[currentPerson1.Id] = true;
                
                continue;
            }
            
            var freePerson1WishlistIds = group1Wishlists.Find(w => w.EmployeeId == freePerson1.Id)?.DesiredEmployees;
            Debug.Assert(freePerson1WishlistIds != null, nameof(freePerson1WishlistIds) + " == null");
            
            var newFreePerson1WishlistIds = freePerson1WishlistIds.Where((v, id) => id != Array.IndexOf(freePerson1WishlistIds, mostWantedPerson2Id)).ToArray();

            var newFreePerson1Wishlist = new Wishlist(freePerson1.Id, newFreePerson1WishlistIds);
            
            var wishlistIdx = group1Wishlists.FindIndex(w => w.EmployeeId == freePerson1.Id);
            group1Wishlists[wishlistIdx] = newFreePerson1Wishlist;
        }

        return teams;
    }

    private static double CountSatisfactionIndex(List<Employee> teamLeadsList, List<Employee> juniorsList, 
        List<Wishlist> teamLeadsWishlistsList, List<Wishlist> juniorsWishlistsList, List<Team> teams)
    {
        var teamLeadsSatisfactionIndexes = new int[teamLeadsList.Count];
        var juniorsSatisfactionIndexes = new int[juniorsList.Count];
        
        foreach (var (teamLead, junior) in teams)
        {
            var teamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == teamLead.Id)?.DesiredEmployees;
            Debug.Assert(teamLeadWishlistIds != null, nameof(teamLeadWishlistIds) + " == null");
            var juniorWishlistIds = juniorsWishlistsList.Find(w => w.EmployeeId == junior.Id)?.DesiredEmployees;
            Debug.Assert(juniorWishlistIds != null, nameof(juniorWishlistIds) + " == null");
            
            var teamLeadSatisfactionIndex = teamLeadsWishlistsList.Count - Array.FindIndex(teamLeadWishlistIds, j => j == junior.Id);
            var juniorSatisfactionIndex = juniorsWishlistsList.Count - Array.FindIndex(juniorWishlistIds, t => t == teamLead.Id);

            teamLeadsSatisfactionIndexes[teamLead.Id - 1] = teamLeadSatisfactionIndex;
            juniorsSatisfactionIndexes[junior.Id - 1] = juniorSatisfactionIndex;
        }
        

        return (double)(juniorsSatisfactionIndexes.Sum() + teamLeadsSatisfactionIndexes.Sum()) / (juniorsSatisfactionIndexes.Length + teamLeadsSatisfactionIndexes.Length);
    }
    
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = new List<Employee>(teamLeads);
        var juniorsList = new List<Employee>(juniors);
        
        var teamLeadsWishlistsList = new List<Wishlist>(teamLeadsWishlists);
        var juniorsWishlistsList = new List<Wishlist>(juniorsWishlists);
        
        var teams1 = PerformAlgorithmWithPriorityToGroup(teamLeadsList, juniorsList, teamLeadsWishlistsList, juniorsWishlistsList, "TeamLeads");
        var teams2 = PerformAlgorithmWithPriorityToGroup(juniorsList, teamLeadsList, juniorsWishlistsList, teamLeadsWishlistsList, "Juniors");

        var result1 = CountSatisfactionIndex(teamLeadsList, juniorsList, teamLeadsWishlistsList, juniorsWishlistsList, teams1);
        var result2 = CountSatisfactionIndex(teamLeadsList, juniorsList, teamLeadsWishlistsList, juniorsWishlistsList, teams2);

        Console.Out.WriteLine(result1 + " " + result2);
        
        return result1 >= result2 ? teams1 : teams2;
    }
}