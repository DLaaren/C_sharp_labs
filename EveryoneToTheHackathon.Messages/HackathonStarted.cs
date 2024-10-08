namespace EveryoneToTheHackathon.Messages;

public class HackathonStarted(int hackathonId)
{
    public int HackathonId { get; set; } = hackathonId;
}