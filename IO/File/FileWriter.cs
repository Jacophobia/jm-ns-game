using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IO.File;

public static class FileWriter
{
    public static void Save(string filepath, string data)
    {
        using var writer = new StreamWriter(filepath);

        writer.WriteAsync(data);
    }

    public static async void Save(string filepath, IEnumerable<IEnumerable<int>> csv)
    {
        await Save(filepath, csv.Select(row => string.Join(",", row.Select(cell => cell.ToString()))));
    }
    
    public static async Task Save(string filepath, IEnumerable<string> csv)
    {
        await using var writer = new StreamWriter(filepath);
        
        foreach (var row in csv)
        {
            await writer.WriteLineAsync(row);
        }
    }
}