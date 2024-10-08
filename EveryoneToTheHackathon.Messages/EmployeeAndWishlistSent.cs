namespace EveryoneToTheHackathon.Messages;

public class EmployeeAndWishlistSent(string message, int id, string title, string name)
{
    public string Message { get; set; } = message;
    public int Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Name { get; set; } = name;
}