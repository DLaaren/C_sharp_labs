using System.Diagnostics;

namespace EveryoneToTheHackathon.DataContracts;

public class ProposeAndRejectAlgorithm : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = new List<Employee>(teamLeads);
        var juniorsList = new List<Employee>(juniors);
        
        var teamLeadsWishlistsList = new List<Wishlist>(teamLeadsWishlists);
        var juniorsWishlistsList = new List<Wishlist>(juniorsWishlists);
        
        var freeTeamLeads = new bool[teamLeadsList.Count + 1];
        var freeJuniors = new bool[juniorsList.Count + 1];
        for (var i = 1; i < freeTeamLeads.Length; i++)
            freeTeamLeads[i] = true;
        for (var i = 1; i < freeJuniors.Length; i++)
            freeJuniors[i] = true;
        
        var teams = new List<Team>();
        
        // TODO use var
        
        // If there is free teamlead
        while (freeTeamLeads.Contains(true))
        {
            Employee? freeTeamLead = teamLeadsList.Find(t => t.Id == Array.FindIndex(freeTeamLeads, f => f));
            Debug.Assert(freeTeamLead != null, nameof(freeTeamLead) + " != null");
            
            Wishlist? freeTeamLeadsWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id);
            Debug.Assert(freeTeamLeadsWishlist != null, nameof(freeTeamLeadsWishlist) + " != null");
            
            int mostWantedJuniorId = freeTeamLeadsWishlist.DesiredEmployees.First();
            
            Employee? mostWantedJunior = juniorsList.Find(j => j.Id == mostWantedJuniorId);
            Debug.Assert(mostWantedJunior != null, nameof(mostWantedJunior) + " != null");
            
            // If the wanted junior is free, then make team with the teamlead and this junior
            if (freeJuniors[mostWantedJuniorId] == true)
            {   
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;
                
                continue;
            }
            
            // Find a team member of most wanted junior
            Employee? currentTeamLead =
                teams.Find(t => t.Junior.Id == mostWantedJuniorId)?.TeamLead;
            Debug.Assert(currentTeamLead != null, nameof(currentTeamLead) + " != null");
            
            Wishlist? wishlistOfMostWantedJunior = juniorsWishlistsList.Find(w => w.EmployeeId == mostWantedJuniorId);
            Debug.Assert(wishlistOfMostWantedJunior != null, nameof(wishlistOfMostWantedJunior) + " != null");
            
            // Find priorities of free teamlead and most wanted junior's team member
            int freeTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, freeTeamLead.Id);
            int currentTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, currentTeamLead.Id);
            
            // If most wanted junior isn't free but free teamlead has the higher priority than most wanted junior's team member
            if (freeJuniors[mostWantedJuniorId] == false &&
                freeTeamLeadPriority > currentTeamLeadPriority)
            {
                // Make team with most wanted junior and free teamlead
                Team? removedTeam = teams.Find(t => t.Junior.Id == mostWantedJuniorId);
                Debug.Assert(removedTeam != null, nameof(removedTeam) + " != null");
                teams.Remove(removedTeam);
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                // And remove them from lists with free ones
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;

                // Remove most wanted junior from ex team member's wishlist
                int[]? currentTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == currentTeamLead.Id)?.DesiredEmployees;
                Debug.Assert(currentTeamLeadWishlistIds != null, nameof(currentTeamLeadWishlistIds) + " != null");
                
                int[] newCurrentTeamLeadWishlistIds = currentTeamLeadWishlistIds.Where(id => id != Array.IndexOf(currentTeamLeadWishlistIds, mostWantedJunior.Id)).ToArray();
                Wishlist newCurrentTeamLeadWishlist = new Wishlist(currentTeamLead.Id, newCurrentTeamLeadWishlistIds);
                teamLeadsWishlistsList[teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == currentTeamLead.Id)] =
                    newCurrentTeamLeadWishlist;
                
                // Mark ex team member as free
                freeTeamLeads[currentTeamLead.Id] = true;
                
                continue;
            }
            
            // Remove most wanted junior from free teamlead's wishlist
            int[]? freeTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id)?.DesiredEmployees;
            Debug.Assert(freeTeamLeadWishlistIds != null, nameof(freeTeamLeadWishlistIds) + " != null");
            
            int[] newFreeTeamLeadWishlistIds = freeTeamLeadWishlistIds.Where((v, id) => id != Array.IndexOf(freeTeamLeadWishlistIds, mostWantedJuniorId)).ToArray();

            Wishlist newFreeTeamLeadWishlist = new Wishlist(freeTeamLead.Id, newFreeTeamLeadWishlistIds);
            
            int wishlistIdx = teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == freeTeamLead.Id);
            teamLeadsWishlistsList[wishlistIdx] = newFreeTeamLeadWishlist;
        }
        
        return teams;
    }
}