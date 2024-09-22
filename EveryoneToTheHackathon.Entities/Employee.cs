using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

[PrimaryKey("Id", "Title")]
public class Employee
{
    public int Id { get; set; }
    public string Title { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    //public int? HackathonId { get; set; }
    public IEnumerable<Hackathon>? Hackathons { get; set; } = new List<Hackathon>();
    
    //public int? TeamId { get; set; }
    public IEnumerable<Team>? Teams { get; set; } = new List<Team>();
    public Wishlist? Wishlist { get; set; }
    
    public Employee() {}

    public Employee(int id, string title, string name)
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