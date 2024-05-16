using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoGame.Singletons;

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

    internal static List<List<CollisionManager.CollisionCheckColumn>> ReadCollisionDataCsv(string filepath)
    {
        return ReadFile(filepath)
            .Select(line => line.Split(','))
            .Select(collisionColumns => collisionColumns
                .Where(collisionColumn => !string.IsNullOrEmpty(collisionColumn))
                .Select(collisionColumn => collisionColumn.Split(':'))
                .Select(collisionColumn => new CollisionManager.CollisionCheckColumn(bool.Parse(collisionColumn[0]), int.Parse(collisionColumn[1])))
                .ToList())
            .ToList();
    }
}