using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EveryoneToTheHackathon.Entities;

[NotMapped]
public class ProposeAndRejectAlgorithm : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, 
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        var teamLeadsList = (List<Employee>)teamLeads;
        var juniorsList = (List<Employee>)juniors;
        
        var teamLeadsWishlistsList = new List<Wishlist>(teamLeadsWishlists);
        var juniorsWishlistsList = new List<Wishlist>(juniorsWishlists);
        
        var freeTeamLeads = new bool[teamLeadsList.Count + 1];
        var freeJuniors = new bool[juniorsList.Count + 1];
        for (var i = 1; i < freeTeamLeads.Length; i++)
            freeTeamLeads[i] = true;
        for (var i = 1; i < freeJuniors.Length; i++)
            freeJuniors[i] = true;
        
        var teams = new List<Team>();
        
        // If there is free teamlead
        while (freeTeamLeads.Contains(true))
        {
            var freeTeamLead = teamLeadsList.Find(t => t.Id == Array.FindIndex(freeTeamLeads, f => f));
            Debug.Assert(freeTeamLead != null, nameof(freeTeamLead) + " != null");
            
            var freeTeamLeadsWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id);
            Debug.Assert(freeTeamLeadsWishlist != null, nameof(freeTeamLeadsWishlist) + " != null");
            
            int mostWantedJuniorId = freeTeamLeadsWishlist.DesiredEmployees.First();
            
            var mostWantedJunior = juniorsList.Find(j => j.Id == mostWantedJuniorId);
            Debug.Assert(mostWantedJunior != null, nameof(mostWantedJunior) + " != null");
            
            // If the wanted junior is free, then make team with the teamlead and this junior
            if (freeJuniors[mostWantedJuniorId] == true)
            {
                Team newTeam = new Team(freeTeamLead, mostWantedJunior);
                teams.Add(newTeam);
                ((List<Team>)freeTeamLead.Teams).Add(newTeam);
                ((List<Team>)mostWantedJunior.Teams).Add(newTeam);
                
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;
                
                continue;
            }
            
            // Find a team member of most wanted junior
            var currentTeamLead =
                teams.Find(t => t.Junior.Id == mostWantedJuniorId)?.TeamLead;
            Debug.Assert(currentTeamLead != null, nameof(currentTeamLead) + " != null");
            
            var wishlistOfMostWantedJunior = juniorsWishlistsList.Find(w => w.EmployeeId == mostWantedJuniorId);
            Debug.Assert(wishlistOfMostWantedJunior != null, nameof(wishlistOfMostWantedJunior) + " != null");
            
            // Find priorities of free teamlead and most wanted junior's team member
            int freeTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, freeTeamLead.Id);
            int currentTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, currentTeamLead.Id);
            
            // If most wanted junior isn't free but free teamlead has the higher priority than most wanted junior's team member
            if (freeJuniors[mostWantedJuniorId] == false &&
                freeTeamLeadPriority > currentTeamLeadPriority)
            {
                // Make team with most wanted junior and free teamlead
                var removedTeam = teams.Find(t => t.Junior.Id == mostWantedJuniorId);
                Debug.Assert(removedTeam != null, nameof(removedTeam) + " != null");
                teams.Remove(removedTeam);

                Team newTeam = new Team(freeTeamLead, mostWantedJunior);
                teams.Add(newTeam);
                //freeTeamLead.Team = newTeam;
                //mostWantedJunior.Team = newTeam;
                ((List<Team>)freeTeamLead.Teams).Add(newTeam);
                ((List<Team>)mostWantedJunior.Teams).Add(newTeam);
                
                // And remove them from lists with free ones
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;

                // Remove most wanted junior from ex team member's wishlist
                var currentTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == currentTeamLead.Id)?.DesiredEmployees;
                Debug.Assert(currentTeamLeadWishlistIds != null, nameof(currentTeamLeadWishlistIds) + " != null");
                
                var newCurrentTeamLeadWishlistIds = currentTeamLeadWishlistIds.Where(id => id != Array.IndexOf(currentTeamLeadWishlistIds, mostWantedJunior.Id)).ToArray();
                var newCurrentTeamLeadWishlist = new Wishlist(currentTeamLead.Id, "TeamLead", newCurrentTeamLeadWishlistIds);
                teamLeadsWishlistsList[teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == currentTeamLead.Id)] =
                    newCurrentTeamLeadWishlist;
                
                // Mark ex team member as free
                freeTeamLeads[currentTeamLead.Id] = true;
                
                continue;
            }
            
            // Remove most wanted junior from free teamlead's wishlist
            var freeTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id)?.DesiredEmployees;
            Debug.Assert(freeTeamLeadWishlistIds != null, nameof(freeTeamLeadWishlistIds) + " != null");
            
            var newFreeTeamLeadWishlistIds = freeTeamLeadWishlistIds.Where((v, id) => id != Array.IndexOf(freeTeamLeadWishlistIds, mostWantedJuniorId)).ToArray();

            var newFreeTeamLeadWishlist = new Wishlist(freeTeamLead.Id, "TeamLead", newFreeTeamLeadWishlistIds);
            
            int wishlistIdx = teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == freeTeamLead.Id);
            teamLeadsWishlistsList[wishlistIdx] = newFreeTeamLeadWishlist;
        }
        
        return teams;
    }
}