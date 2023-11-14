using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IO.File;

public static class FileWriter
{
    public static void Save(string filepath, string data)
    {
        using var writer = new StreamWriter(filepath);

        writer.WriteAsync(data);
    }

    public static void Save(string filepath, IEnumerable<IEnumerable<int>> csv)
    {
        Save(filepath, csv.Select(row => string.Join(",", row.Select(cell => cell.ToString()))));
    }
    
    public static void Save(string filepath, IEnumerable<string> csv)
    {
        using var writer = new StreamWriter(filepath);
        
        foreach (var row in csv)
        {
            writer.WriteLine(row);
        }
    }
}