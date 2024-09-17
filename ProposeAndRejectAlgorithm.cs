namespace lab1;

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
        
        // If there is free teamlead
        while (freeTeamLeads.Contains(true))
        {
            Employee freeTeamLead = teamLeadsList.Find(t => t.Id == Array.FindIndex(freeTeamLeads, f => f));
            
            var freeTeamLeadsWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id);
            
            int mostWantedJuniorId = freeTeamLeadsWishlist.DesiredEmployees.First();
            Employee mostWantedJunior = juniorsList.Find(j => j.Id == mostWantedJuniorId);

            // If the wanted junior is free, then make team with the teamlead and this junior
            if (freeJuniors[mostWantedJuniorId] == true)
            {   
                //Console.WriteLine("Junior is free");
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                // Remove the teamlead and the junior from list with free ones
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;
                continue;
            }
            
            // Find a team member of most wanted junior
            Employee currentTeamLead =
                teams.Find(t => t.Junior.Id == mostWantedJuniorId).TeamLead;
            Wishlist wishlistOfMostWantedJunior = juniorsWishlistsList.Find(w => w.EmployeeId == mostWantedJuniorId);

            // Find priorities of free teamlead and most wanted junior's team member
            int freeTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, freeTeamLead.Id);
            int currentTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, currentTeamLead.Id);

            // If most wanted junior isn't free but free teamlead has the higher priority than most wanted junior's team member
            if (freeJuniors[mostWantedJuniorId] == false &&
                freeTeamLeadPriority > currentTeamLeadPriority)
            {
                // Make team with most wanted junior and free teamlead
                teams.Remove(teams.Find(t => t.Junior.Id == mostWantedJuniorId));
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                // And remove them from lists with free ones
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;

                // Remove most wanted junior from ex team member's wishlist
                var currentTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == currentTeamLead.Id).DesiredEmployees;
                var newCurrentTeamLeadWishlistIds = currentTeamLeadWishlistIds.Where(id => id != Array.IndexOf(currentTeamLeadWishlistIds, mostWantedJunior.Id)).ToArray();
                var newCurrentTeamLeadWishlist = new Wishlist(currentTeamLead.Id, newCurrentTeamLeadWishlistIds);
                teamLeadsWishlistsList[teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == currentTeamLead.Id)] =
                    newCurrentTeamLeadWishlist;
                
                // Mark ex team member as free
                freeTeamLeads[currentTeamLead.Id] = true;
                
                continue;
            }
            
            // Remove most wanted junior from free teamlead's wishlist
            var freeTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id).DesiredEmployees;
            var newFreeTeamLeadWishlistIds = freeTeamLeadWishlistIds.Where((v, id) => id != Array.IndexOf(freeTeamLeadWishlistIds, mostWantedJuniorId)).ToArray();
            var newFreeTeamLeadWishlist = new Wishlist(freeTeamLead.Id, newFreeTeamLeadWishlistIds);
            var idxx = teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == freeTeamLead.Id);
            teamLeadsWishlistsList[idxx] = newFreeTeamLeadWishlist;
        }
        
        return teams;
    }
}
