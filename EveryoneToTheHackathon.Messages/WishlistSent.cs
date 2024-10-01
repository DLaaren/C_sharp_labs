using EveryoneToTheHackathon.Entities;

namespace EveryoneToTheHackathon.Messages;

public class WishlistSent {
    
    public WishlistSent() {}
    
    public WishlistSent(int employeeId, string employeeTitle, int[] desiredEmployees)
    {
        EmployeeId = employeeId;
        EmployeeTitle = employeeTitle;
        DesiredEmployees = desiredEmployees;
    }

    public int EmployeeId { get; set; }
    public string EmployeeTitle { get; set; } 
    public int[] DesiredEmployees { get; set; }
}