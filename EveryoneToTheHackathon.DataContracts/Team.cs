using System.ComponentModel.DataAnnotations;

namespace EveryoneToTheHackathon.DataContracts;

public class Team
{
    [Key]
    [Required]
    public int Id { get; init; }
    [Required]
    public Employee TeamLead { get; init; }
    [Required]
    public Employee Junior { get; init; }

    public Team() {}
    
    public Team(Employee teamLead, Employee junior)
    {
        TeamLead = teamLead;
        Junior = junior;
    }
}   
