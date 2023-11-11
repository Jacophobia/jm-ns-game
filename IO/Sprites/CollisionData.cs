using System.Collections.Generic;
using System.IO;
using System.Linq;
using IO.Extensions;
using IO.File;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Sprites;

public class CollisionData
{
    // At some point, it would be good to make a custom data structure 
    // which could order points so that you can perform a more specific
    // search.
    private static IDictionary<string, ISet<Vector2>> _collisionData;
    private ISet<Vector2> Data
    {
        get => _collisionData[_name];
        set => _collisionData[_name] = value;
    }

    private readonly string _name;

    public CollisionData(Texture2D texture)
    {
        _name = texture.Name;
        
        _collisionData ??= new Dictionary<string, ISet<Vector2>>();
        
        if (_collisionData.ContainsKey(_name))
        {
            return;
        }

        _collisionData.TryAdd(_name, new HashSet<Vector2>());

        if (Directory.Exists(texture.GetCollisionDataFilepath()))
        {
            Load(texture.GetCollisionDataFilepath());
            return;
        }
        
        // Get the pixel data from the textures
        var data1 = new Color[texture.Width * texture.Height];
        texture.GetData(data1);

        for (var x = 0; x < texture.Width; x++)
        for (var y = 0; y < texture.Height; y++)
        {
            var color = data1.Get(x, y, texture.Width);

            if (color.A == byte.MaxValue && HasTransparentNeighbor(data1, x, y, texture.Width, texture.Height, (texture.Width + texture.Height) / 50))
            {
                Data.Add(new Vector2(x, y));
            }
        }

        Save(texture.GetCollisionDataFilepath());
    }
    
    private static bool HasTransparentNeighbor(Color[] image, int x, int y, int width, int height, int range)
    {
        for (var dx = x - range; dx < x + range; dx++)
        for (var dy = y - range; dy < y + range; dy++)
        {
            if (image.TryGet(dx, dy, width, height, out var color) && color.A != byte.MaxValue)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool IsCollidableCoord(Vector2 coordinate)
    {
        return Data.Contains(coordinate);
    }

    public void Save(string filepath)
    {
        FileWriter.Save(filepath, Data.Select(line => $"{line.X},{line.Y}"));
    }

    public void Load(string filepath)
    {
        Data?.Clear();
        Data = FileReader
            .ReadCsv(filepath)
            .Select(line => new Vector2(line[0], line[1]))
            .ToHashSet();
    }
}