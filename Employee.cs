using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsu.HackathonProblem.Contracts;

public record Employee(int Id, string Name)
{
    public Wishlist MakeWishlist(IEnumerable<Employee> employees)
    {
        return new Wishlist(Id, employees.
            Select(e => e.Id).
            OrderBy( _ => Random.Shared.Next()).
            ToArray());
    }
}