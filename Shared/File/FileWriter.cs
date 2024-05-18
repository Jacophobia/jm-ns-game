using System.Collections.Generic;
using System.IO;
using Shared.Singletons;

namespace Shared.File;

internal static class FileWriter
{
    internal static void Save(string filepath, string data)
    {
        using var writer = new StreamWriter(filepath);

        writer.WriteAsync(data);
    }

    internal static void SaveCollisionData(string filepath, IEnumerable<IEnumerable<CollisionManager.CollisionCheckColumn>> csv)
    {
        using var writer = new StreamWriter(filepath);

        foreach (var row in csv)
            writer.WriteLine(string.Join(',', row));
    }
}