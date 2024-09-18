using EveryoneToTheHackathon.DataContracts;
using Microsoft.Extensions.Hosting;

namespace EveryoneToTheHackathon.Host;

public static class CsvParser
{
    public static IEnumerable<Employee> ParseCsvFileWithEmployees(string filePath)
    {
        List<Employee> employeesList = new();
        foreach (string line in File.ReadAllLines(filePath))
        {
            string[] tokens = line.Split(";");
            // skip first line with attributes
            if (!Int32.TryParse(tokens[0], out var id))
                continue;
            string name = tokens[1];
            employeesList.Add(new Employee(id, name));
        }
        return employeesList;
    }
}