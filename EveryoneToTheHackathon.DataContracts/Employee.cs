using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EveryoneToTheHackathon.DataContracts;

[PrimaryKey("Id", "Title")]
public class Employee(int id, EmployeeTitle title, string name)
{
    [Key]
    [Required]
    public int Id { get; init; } = id;
    [Key]
    [Required]
    public EmployeeTitle Title { get; init; } = title;
    [Required]
    [MaxLength(50)]
    public string Name { get; init; } = name;

    public Wishlist MakeWishlist(IEnumerable<Employee> employees)
    {
        return new Wishlist(Id, Title, employees.
            Select(e => e.Id).
            OrderBy( _ => Random.Shared.Next()).
            ToArray());
    }
}