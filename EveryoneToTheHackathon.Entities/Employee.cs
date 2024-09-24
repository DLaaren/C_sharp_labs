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
    public IEnumerable<Wishlist>?  Wishlists { get; set; }
    public IEnumerable<Team>? Teams { get; set; }
    public IEnumerable<Hackathon>? Hackathons { get; set; }
    /* Navigation properties */
    
    public Employee() {}
    
    public Employee(int id, string title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
        Teams = new List<Team>();
    }
    
    public Wishlist MakeWishlist(IEnumerable<Employee> employees)
    {
        return new Wishlist(Id, Title, employees.
            Select(e => e.Id).
            OrderBy( _ => Random.Shared.Next()).
            ToArray());
    }
}