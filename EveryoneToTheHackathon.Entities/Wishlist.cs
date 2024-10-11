using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

public class Wishlist
{
    /* Public properties for database */
    [Key]
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeTitle { get; set; }
    [Required] 
    public int[] DesiredEmployees { get; set; } = [];
    /* Public properties for database */
    
    /* Navigation properties */
    public Employee? Employee { get; set; }
    public int? HackathonId { get; set; }
    public Hackathon? Hackathon { get; set; }
    /* Navigation properties */
    
    public Wishlist() {}

    public Wishlist(int employeeId, string title, int[] desiredEmployees)
    {
        EmployeeId = employeeId;
        EmployeeTitle = title;
        DesiredEmployees = desiredEmployees;
    }
}
