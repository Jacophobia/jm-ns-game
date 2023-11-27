using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoGame.File;

internal static class FileReader
{
    internal static IEnumerable<string> GetImages(string subfolder = "")
    {
        const string contentRoot = "Content/";

        if (!Directory.Exists(contentRoot + subfolder)) return new List<string>();

        return Directory
            .GetFiles(contentRoot + subfolder)
            .Where(file => Path.GetExtension(file) != ".csv")
            .Select(Path.GetFileNameWithoutExtension);
    }

    internal static IEnumerable<string> ReadFile(string filepath)
    {
        using var reader = new StreamReader(filepath);

        while (!reader.EndOfStream) yield return reader.ReadLine();
    }

    internal static IEnumerable<int[]> ReadCsv(string filepath)
    {
        return ReadFile(filepath)
            .Select(line => line.Split(','))
            .Select(elements => elements.Select(int.Parse).ToArray());
    }
}