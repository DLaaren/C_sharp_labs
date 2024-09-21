using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

[PrimaryKey("EmployeeId", "EmployeeTitle")]
public class Wishlist
{ 
    public int EmployeeId { get; init; }
    public EmployeeTitle EmployeeTitle { get; init; }
    
    public Employee? Employee { get; init; }
    
    [Required]
    public int[] DesiredEmployees { get; init; }
    
    public Wishlist() {}

    public Wishlist(int employeeId, EmployeeTitle title, int[] desiredEmployees)
    {
        EmployeeId = employeeId;
        EmployeeTitle = title;
        DesiredEmployees = desiredEmployees;
    }
}
