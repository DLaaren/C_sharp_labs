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

    private Hackathon? _hackathon;

    public double CalculationMeanSatisfactionIndex()
    {
        var meanSatisfactionIndex = HrDirector.CalculateMeanSatisfactionIndex(
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.TeamLead)).ToList(),
            Wishlists!.Where(w => w.EmployeeTitle.Equals(EmployeeTitle.Junior)).ToList(),
            Teams!);
        return meanSatisfactionIndex;
    }

    public int StartHackathon()
    {
        _hackathon = new Hackathon();
        HackathonRepository.AddHackathon(_hackathon);
        return _hackathon.Id;
    } 
    
    public void SaveHackathon(double meanSatisfactionIndex)
    {
        Debug.Assert(_hackathon != null);
        _hackathon.MeanSatisfactionIndex = meanSatisfactionIndex;
        _hackathon.Employees = Employees;
        _hackathon.Wishlists = Wishlists;
        _hackathon.Teams = Teams;
        Teams!.ForEach(t =>
        {
            t.Hackathon = _hackathon;
            t.HackathonId = _hackathon.Id;
        });
        Employees!.ForEach(e => ((List<Team>)e.Teams!).AddRange( Teams.FindAll(t => t.TeamLead.Equals(e) || t.Junior.Equals(e))) );
        
        HackathonRepository.UpdateHackathon(_hackathon);
    }
}