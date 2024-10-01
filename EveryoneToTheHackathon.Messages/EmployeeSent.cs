namespace EveryoneToTheHackathon.Messages;

public class EmployeeSent
{
    public EmployeeSent() {}

    public EmployeeSent(int id, string title, string name)
    {
        Id = id;
        Title = title;
        Name = name;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Name { get; set; }
}