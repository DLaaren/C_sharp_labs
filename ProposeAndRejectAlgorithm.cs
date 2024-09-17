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
        
        // Algo
        // https://neerc.ifmo.ru/wiki/index.php?title=%D0%97%D0%B0%D0%B4%D0%B0%D1%87%D0%B0_%D0%BE%D0%B1_%D1%83%D1%81%D1%82%D0%BE%D0%B9%D1%87%D0%B8%D0%B2%D0%BE%D0%BC_%D0%BF%D0%B0%D1%80%D0%BE%D1%81%D0%BE%D1%87%D0%B5%D1%82%D0%B0%D0%BD%D0%B8%D0%B8

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
            //Console.WriteLine("\nGot free teamlead: {0}", freeTeamLead);
            
            var freeTeamLeadsWishlist = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id);
            /*Console.Write("His wishlist: ");
            foreach (var jId in freeTeamLeadsWishlist.DesiredEmployees)
                Console.Write("{0}; ", jId);
            Console.WriteLine();*/
            
            int mostWantedJuniorId = freeTeamLeadsWishlist.DesiredEmployees.First();
            Employee mostWantedJunior = juniorsList.Find(j => j.Id == mostWantedJuniorId);
            
            //Console.WriteLine("Most wanted junior: {0}", mostWantedJuniorId);

            // If the wanted junior is free, then make team with the teamlead and this junior
            if (freeJuniors[mostWantedJuniorId] == true)
            {   
                //Console.WriteLine("Junior is free");
                teams.Add(new Team(freeTeamLead, mostWantedJunior));
                
                // Remove the teamlead and the junior from list with free ones
                freeTeamLeads[freeTeamLead.Id] = false;
                freeJuniors[mostWantedJuniorId] = false;

                /*Console.WriteLine("Current teams: ");
                foreach (var team in teams) 
                    Console.WriteLine("TL: {0} && J: {1}", team.TeamLead.Id, team.Junior.Id);
                Console.WriteLine();*/
                continue;
            }
            
            // Find a team member of most wanted junior
            Employee currentTeamLead =
                teams.Find(t => t.Junior.Id == mostWantedJuniorId).TeamLead;
            Wishlist wishlistOfMostWantedJunior = juniorsWishlistsList.Find(w => w.EmployeeId == mostWantedJuniorId);

            // Find priorities of free teamlead and most wanted junior's team member
            int freeTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, freeTeamLead.Id);
            int currentTeamLeadPriority = 20 - Array.IndexOf(wishlistOfMostWantedJunior.DesiredEmployees, currentTeamLead.Id);
            
            //Console.WriteLine("free teamlead's priority = {1}; current team member's priority = {0}", currentTeamLeadPriority, freeTeamLeadPriority);

            // If most wanted junior isn't free but free teamlead has the higher priority than most wanted junior's team member
            if (freeJuniors[mostWantedJuniorId] == false &&
                freeTeamLeadPriority > currentTeamLeadPriority)
            {
                /*Console.WriteLine("Junior is busy");
                Console.Write("His wishlist: ");
                foreach (var tlId in wishlistOfMostWantedJunior.DesiredEmployees)
                    Console.Write("{0}; ", tlId);
                Console.WriteLine();*/
                
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
                
                /*Console.WriteLine("Current teams: ");
                foreach (var team in teams) 
                    Console.WriteLine("TL: {0} && J: {1}", team.TeamLead.Id, team.Junior.Id);
                Console.WriteLine();*/
                continue;
            }
            
            /*Console.WriteLine("DEBUG");*/
            // Remove most wanted junior from free teamlead's wishlist
            var freeTeamLeadWishlistIds = teamLeadsWishlistsList.Find(w => w.EmployeeId == freeTeamLead.Id).DesiredEmployees;
            
            /*Console.WriteLine("freeTeamLeadWishlistIds: ");
            foreach (var id in freeTeamLeadWishlistIds) 
                Console.Write("id: {0}; ", id);
            Console.WriteLine();*/
            
            var newFreeTeamLeadWishlistIds = freeTeamLeadWishlistIds.Where((v, id) => id != Array.IndexOf(freeTeamLeadWishlistIds, mostWantedJuniorId)).ToArray();
            
            /*Console.WriteLine("newFreeTeamLeadWishlistIds: ");
            foreach (var id in newFreeTeamLeadWishlistIds) 
                Console.Write("id: {0}; ", id);
            Console.WriteLine();*/

            var newFreeTeamLeadWishlist = new Wishlist(freeTeamLead.Id, newFreeTeamLeadWishlistIds);
            
            var idxx = teamLeadsWishlistsList.FindIndex(w => w.EmployeeId == freeTeamLead.Id);
            teamLeadsWishlistsList[idxx] = newFreeTeamLeadWishlist;
            
            /*Console.WriteLine("teamLeadsWishlistsList[{0}]: = {1}", idxx, teamLeadsWishlistsList[idxx]);
            Console.WriteLine();*/
        }
        
        return teams;
    }
}