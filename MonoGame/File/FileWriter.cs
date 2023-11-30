using System.Collections.Generic;
using System.IO;
using MonoGame.Sprites;

namespace MonoGame.File;

internal static class FileWriter
{
    internal static void Save(string filepath, string data)
    {
        using var writer = new StreamWriter(filepath);

        writer.WriteAsync(data);
    }

    internal static void SaveCollisionData(string filepath, IEnumerable<IEnumerable<CollisionData.CollisionCheckColumn>> csv)
    {
        using var writer = new StreamWriter(filepath);

        foreach (var row in csv)
            writer.WriteLine(string.Join(',', row));
    }
}