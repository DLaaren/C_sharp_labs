using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.Entities;

[PrimaryKey("Id", "Title")]
public class Employee
{
    /* Public properties for database */
    public int Id { get; set; }
    public string Title { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    /* Public properties for database */
    
    /* Navigation properties */
    public IEnumerable<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public IEnumerable<Team> Teams { get; set; } = new List<Team>();
    public IEnumerable<Hackathon> Hackathons { get; set; } = new List<Hackathon>();
    /* Navigation properties */
    
    public Employee() {}
    
    public Employee(int id, string title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
        // Wishlists = new List<Wishlist>();
        // Teams = new List<Team>();
        // Hackathons = new List<Hackathon>();
    }
    
    public Wishlist MakeWishlist(IEnumerable<Employee> employees)
    {
        return new Wishlist(Id, Title, employees.
            Select(e => e.Id).
            OrderBy( _ => Random.Shared.Next()).
            ToArray());
    }
}