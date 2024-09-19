using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.DataContracts;

[PrimaryKey("EmployeeId", "EmployeeTitle")]
public class Wishlist
{ 
    [Key]
    [Required]
    public int EmployeeId { get; init; }
    [Key]
    [Required]
    public EmployeeTitle EmployeeTitle { get; init; }
    [Required]
    public int[] DesiredEmployees { get; init; }
    
    public Wishlist() {}

    public Wishlist(int employeeId, EmployeeTitle title, int[] desiredEmployees)
    {
        EmployeeId = employeeId;
        EmployeeTitle = title;
        DesiredEmployees = desiredEmployees.ToArray();
    }
}
