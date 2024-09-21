using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

[PrimaryKey("Id", "Title")]
public class Employee
{
    public int Id { get; init; }
    public EmployeeTitle Title { get; init; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; init; }
    
    public int? HackathonId { get; set; } // внешний ключ
    public Hackathon? Hackathon { get; set; } // навигационное свойство
    
    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    public Wishlist? Wishlist { get; set; }
    
    public Employee() {}

    public Employee(int id, EmployeeTitle title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
    }
    
    public Wishlist MakeWishlist(IEnumerable<Employee> employees)
    {
        return new Wishlist(Id, Title, employees.
            Select(e => e.Id).
            OrderBy( _ => Random.Shared.Next()).
            ToArray());
    }
}