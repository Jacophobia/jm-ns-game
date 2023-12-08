using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.File;
using MonoGame.Output;

namespace MonoGame.Collision;

public class CollisionData
{
    private static IDictionary<string, List<List<CollisionCheckColumn>>> _collisionData;

    private readonly string _name;
    private readonly Rectangle _bounds;

    internal CollisionData(Texture2D texture)
    {
        _name = texture.Name;
        _bounds = texture.Bounds;

        _collisionData ??= new Dictionary<string, List<List<CollisionCheckColumn>>>();

        if (_collisionData.ContainsKey(_name)) return;

        _collisionData.TryAdd(_name, new List<List<CollisionCheckColumn>>());

        var relativePath = texture.GetCollisionDataFilepath();
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            Load(fullPath);
            return;
        }
        
        // Get the pixel data from the textures
        var data1 = new Color[texture.Width * texture.Height];
        texture.GetData(data1);


        var collisionData = new List<List<CollisionCheckColumn>>();
        for (var x = 0; x < texture.Width; x++)
        {
            var column = new List<CollisionCheckColumn>();
            for (var y = 0; y < texture.Height; y++)
            {
                var isCollidable = data1.Get(x, y, texture.Width).A == byte.MaxValue;
                if (column.Count == 0 || column.Last().IsCollidable != isCollidable)
                    column.Add(new CollisionCheckColumn(isCollidable));
                column.Last().Count++;
            }
            collisionData.Add(column);
        }
        
        Data = collisionData;
        
        Save(texture.GetCollisionDataFilepath());
    }

    private List<List<CollisionCheckColumn>> Data
    {
        get => _collisionData[_name];
        set => _collisionData[_name] = value;
    }

    private bool IsCollidableCoord(Vector2 coordinate)
    {
        if (coordinate.X >= Data.Count)
            return false;
        
        var column = Data[(int)Math.Round(coordinate.X)];
        var count = 0;
        
        foreach (var check in column)
        {
            count += check.Count;
            if (count >= coordinate.Y) 
                return check.IsCollidable;
        }

        return false;
    }
    internal bool Collides(Rectangle lhsDestination, CollisionData rhs, Rectangle rhsDestination, out Rectangle? overlap)
    {
        overlap = Rectangle.Intersect(lhsDestination, rhsDestination);
        
        if (overlap is { IsEmpty: true })
            return false;

        // Create rectangles for the entities' collision areas
        var rect1 = lhsDestination;
        var rect2 = rhsDestination;

        // Get the textures of the entities
        var texture1 = _bounds;
        var texture2 = rhs._bounds;

        // var overlapLeft = (int)Math.Round((double)((overlap.Value.X - rect1.Left) * texture1.Width) / rect1.Width);
        // var overlapTop = (int)Math.Round((double)((overlap.Value.Y - rect1.Top) * texture1.Height) / rect1.Height);
        // var overlapRight = (int)Math.Round((double)((overlap.Value.Right - rect1.Left) * texture1.Width) / rect1.Width);
        // var overlapBottom = (int)Math.Round((double)((overlap.Value.Bottom - rect1.Top) * texture1.Height) / rect1.Height);

        // Iterate through the intersection area and check for pixel-perfect collision
        for (var x = overlap.Value.Left; x <= overlap.Value.Right; x++)
        for (var y = overlap.Value.Top; y <= overlap.Value.Bottom; y++)
        {
            // Calculate the pixel coordinates within the textures
            var texCoord1 = new Vector2(
                (float)(x - rect1.Left) * texture1.Width / rect1.Width,
                (float)((y - rect1.Top) * texture1.Height) / rect1.Height);
            var texCoord2 = new Vector2(
                (float)((x - rect2.Left) * texture2.Width) / rect2.Width,
                (float)((y - rect2.Top) * texture2.Height) / rect2.Height);

            // Check if the pixels at the current position are not transparent for both entities
            if (IsCollidableCoord(texCoord1) && rhs.IsCollidableCoord(texCoord2))
            {
                return true;
            }
        }
        
        return false;
    }

    private void Save(string filepath)
    {
        FileWriter.SaveCollisionData(filepath, Data);
    }

    private void Load(string filepath)
    {
        Data?.Clear();
        Data = FileReader.ReadCollisionDataCsv(filepath);
    }

    internal class CollisionCheckColumn
    {
        public readonly bool IsCollidable;
        public int Count;
        
        public CollisionCheckColumn(bool isCollidable, int count = 0)
        {
            IsCollidable = isCollidable;
            Count = count;
        }

        public override string ToString()
        {
            return $"{IsCollidable}:{Count}";
        }
    }
}