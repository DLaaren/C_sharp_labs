using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

[PrimaryKey("EmployeeId", "EmployeeTitle", "HackathonId")]
public class Wishlist
{ 
    public int EmployeeId { get; set; }
    public string EmployeeTitle { get; set; }
    
    public Employee? Employee { get; set; }
    
    [Required]
    public int[] DesiredEmployees { get; set; }
    
    public int HackathonId { get; set; } // внешний ключ
    public Hackathon? Hackathon { get; set; } // навигационное свойство
    
    public Wishlist() {}

    public Wishlist(int employeeId, string title, int[] desiredEmployees)
    {
        EmployeeId = employeeId;
        EmployeeTitle = title;
        DesiredEmployees = desiredEmployees;
    }
}
