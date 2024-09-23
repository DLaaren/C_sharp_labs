using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EveryoneToTheHackathon.Entities;

public class Team
{
    /* Public properties for database */
    [Key]
    public int Id { get; set; }
    
    /* Public properties for database */

    /* Navigation properties */
    public int? HackathonId { get; set; }
    public Hackathon? Hackathon { get; set; }
    public List<Employee>? Employees { get; set; }
    /* Navigation properties */
    
    /* Private properties for inner logic */
    [NotMapped] 
    public Employee TeamLead { get; set; }
    [NotMapped] 
    public Employee Junior { get; set; }
    /* Private properties for inner logic */
    
    public Team() {}
    
    public Team(Employee teamLead, Employee junior)
    {
        TeamLead = teamLead;
        Junior = junior;
    }
}   
