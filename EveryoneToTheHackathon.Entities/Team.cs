using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveryoneToTheHackathon.Entities;

public class Team
{
    [Key]
    public int Id { get; init; }

    [NotMapped]
    public Employee TeamLead;
    [NotMapped]
    public Employee Junior;
    
    public int HackathonId { get; set; } // внешний ключ
    public Hackathon? Hackathon { get; set; } // навигационное свойство
    
    public List<Employee> Employees { get; init; }

    public Team() {}
    
    public Team(Employee teamLead, Employee junior)
    {
        TeamLead = teamLead;
        Junior = junior;
    }
}   
