using System.Diagnostics;
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
        Hackathon hackathon = new Hackathon
        {
            Employees = Employees
        };
        HackathonRepository.AddHackathon(hackathon);
        
        hackathon.Employees = Employees;
        hackathon.Wishlists = Wishlists;
        hackathon.Teams = Teams;
        hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        
        ((List<Team>)hackathon.Teams!).ForEach(t =>
        {
            t.Hackathon = hackathon;
            t.HackathonId = hackathon.Id;
        });

        ((List<Employee>)hackathon.Employees!).
            ForEach(e => 
                ((List<Team>)e.Teams!).
                    AddRange( ((List<Team>)hackathon.Teams).
                    FindAll(t => (t.TeamLead.Id.Equals(e.Id) && t.TeamLead.Title.Equals(e.Title)) || (t.Junior.Id.Equals(e.Id) && t.Junior.Title.Equals(e.Title))
                    )));
        
        
        HackathonRepository.UpdateHackathon(hackathon);
    }
}