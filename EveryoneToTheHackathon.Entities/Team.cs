using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveryoneToTheHackathon.Entities;

public class Team
{
    [Key]
    public int Id { get; set; }

    [NotMapped] 
    public Employee TeamLead { get; set; }

    [NotMapped] 
    public Employee Junior { get; set; }

    public int? HackathonId { get; set; } // внешний ключ
    public Hackathon? Hackathon { get; set; } // навигационное свойство
    
    public List<Employee> Employees { get; set; }

    public Team() {}
    
    public Team(Employee teamLead, Employee junior)
    {
        TeamLead = teamLead;
        Junior = junior;
    }
}   
