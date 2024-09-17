namespace lab1;

public class Hackathon
{
    private List<Employee> _juniors = new();
    private List<Employee> _teamLeads = new();
    private HRManager _hrManager;
    private HRDirector _hrDirector;
    
    private List<Wishlist> _juniorsWishlists;
    private List<Wishlist> _teamLeadsWishlists;
    private List<Team> _teams;
    
    private double _meanSatisfactionIndex = -1;
    
    public double MeanSatisfactionIndex { get; private set; }
    
    public Hackathon(HRManager hrManager, HRDirector hrDirector)
    {
        _hrManager = hrManager;
        _hrDirector = hrDirector;
        ParseCsvFileWithEmployees("Resources/Juniors20.csv", _juniors);
        ParseCsvFileWithEmployees("Resources/Teamleads20.csv", _teamLeads);
    }
    
    private void ParseCsvFileWithEmployees(string filePath, IEnumerable<Employee> employees)
    {
        var employeesList = (List<Employee>)employees;
        foreach (string line in File.ReadAllLines(filePath))
        {
            string[] tokens = line.Split(";");
            // skip first line with attributes
            if (!Int32.TryParse(tokens[0], out var id))
                continue;
            string name = tokens[1];
            employeesList.Add(new Employee(id, name));
        }
    }

    public void HoldEvent()
    {
        _juniorsWishlists = new();
        _teamLeadsWishlists = new();
        
        foreach (var junior in _juniors)
            _juniorsWishlists.Add(junior.MakeWishlist(_teamLeads));
        foreach (var teamlead in _teamLeads)
            _teamLeadsWishlists.Add(teamlead.MakeWishlist(_juniors));
        
        var teams1 = (List<Team>)_hrManager.BuildTeams(_teamLeads, _juniors, _teamLeadsWishlists, _juniorsWishlists);
        double idx1 = _hrDirector.CalculateMeanSatisfactionIndex(_teamLeadsWishlists, _juniorsWishlists, teams1);
        
        var teams2 = (List<Team>)_hrManager.BuildTeams(_juniors, _teamLeads, _juniorsWishlists, _teamLeadsWishlists);
        double idx2 = _hrDirector.CalculateMeanSatisfactionIndex(_juniorsWishlists, _teamLeadsWishlists, teams2);
        
        /*Console.WriteLine("is teams different: {0}", teams1.Equals(teams2));
        Console.WriteLine("idx1 {0}; idx2 {1}", idx1, idx2);*/
        
        _teams = idx1 > idx2 ? teams1 : teams2;
        MeanSatisfactionIndex = idx1 > idx2 ? idx1 : idx2;
    }
}