namespace Nsu.HackathonProblem.Contracts;

public class Program
{
    static void Main(string[] args)
    {
        int hackathonNumber = 1000;
        double meanSatisfactionIndexForAllRounds = 0;
        Hackathon hackathon = new Hackathon();
        for (int i = 1; i <= hackathonNumber; i++)
        {
            hackathon.HoldEvent();
            Console.WriteLine("Mean satisfaction index for {0}th round = {1}", 
                i.ToString(), hackathon.MeanSatisfactionIndex);
            meanSatisfactionIndexForAllRounds += hackathon.MeanSatisfactionIndex;
        }

        Console.WriteLine(
            "Mean satisfaction index for all rounds = " + meanSatisfactionIndexForAllRounds / hackathonNumber);
    }
}
