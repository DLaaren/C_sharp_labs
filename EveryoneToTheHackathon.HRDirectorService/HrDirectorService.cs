using EveryoneToTheHackathon.Entities;
using EveryoneToTheHackathon.Repositories;

namespace EveryoneToTheHackathon.HRDirectorService;

public class HrDirectorService(HRDirector hrDirector, IHackathonRepository hackathonRepository)
{
    private HRDirector HrDirector { get; } = hrDirector;
    private IHackathonRepository HackathonRepository { get; } = hackathonRepository;

    public List<Employee>? Employees { get; set; }
    public List<Wishlist>? Wishlists { get; set; }
    public List<Team>? Teams { get; set; }

    public double CalculationMeanSatisfactionIndex()
    {
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)).ToList(),
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)).ToList(),
            Teams!);
        return meanSatisfactionIndex;
    }
    
    public void SaveHackathon(double meanSatisfactionIndex)
    {
        Hackathon hackathon = new Hackathon();
        hackathon.Employees = Employees;
        HackathonRepository.AddHackathon(hackathon);
        
        hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        hackathon.Employees = Employees;
        hackathon.Wishlists = Wishlists;
        hackathon.Teams = Teams;
        Teams!.ForEach(t =>
        {
            t.Hackathon = hackathon;
            t.HackathonId = hackathon.Id;
        });
        Employees!.ForEach(e => ((List<Team>)e.Teams!).AddRange( Teams.FindAll(t => t.TeamLead.Equals(e) || t.Junior.Equals(e))) );
        
        HackathonRepository.UpdateHackathon(hackathon);
    }
}