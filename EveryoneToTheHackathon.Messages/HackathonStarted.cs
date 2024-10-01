namespace EveryoneToTheHackathon.Messages;

public class HackathonStarted
{
    public HackathonStarted() { }
    
    public HackathonStarted(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}