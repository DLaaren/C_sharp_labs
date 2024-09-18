namespace EveryoneToTheHackathon.DataContracts;

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