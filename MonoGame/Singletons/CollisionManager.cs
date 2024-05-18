using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.File;

namespace MonoGame.Singletons;

public class CollisionManager
{
    private static CollisionManager _instance = null;
    
    private readonly IDictionary<string, List<List<CollisionCheckColumn>>> _collisionData;

    private CollisionManager()
    {
        _collisionData = new Dictionary<string, List<List<CollisionCheckColumn>>>();
    }

    public static CollisionManager GetInstance()
    {
        return _instance ??= new CollisionManager();
    }

    public void Load(Texture2D texture)
    {
        if (_collisionData.ContainsKey(texture.Name))
        {
            return;
        }

        var collisionData = new List<List<CollisionCheckColumn>>();

        var relativePath = texture.GetCollisionDataFilepath();
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        if (System.IO.File.Exists(fullPath))
        {
            _collisionData[texture.Name] = FileReader.ReadCollisionDataCsv(fullPath);
            return;
        }
        
        // Get the pixel data from the textures
        var data1 = new Color[texture.Width * texture.Height];
        texture.GetData(data1);
        
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

        _collisionData.TryAdd(texture.Name, collisionData);
    }

    private bool IsCollidableCoord(Texture2D texture, Vector2 coordinate)
    {
        Load(texture);
        
        if (coordinate.X >= _collisionData[texture.Name].Count)
            return false;
        
        var column = _collisionData[texture.Name][(int)Math.Floor(coordinate.X)];
        var count = 0;
        
        foreach (var check in column)
        {
            count += check.Count;
            if (count >= Math.Floor(coordinate.Y)) 
                return check.IsCollidable;
        }

        return false;
    }

    public bool Collides(Texture2D lhs, Rectangle lhsBounds, Texture2D rhs, Rectangle rhsBounds)
    {
        var overlap = Rectangle.Intersect(lhsBounds, rhsBounds);
        
        if (overlap is { IsEmpty: true })
            return false;
        
        // Get the bounds of the entity's textures
        var texture1 = lhs.Bounds;
        var texture2 = rhs.Bounds;
        
        for (var x = overlap.Left; x <= overlap.Right; x++)
        for (var y = overlap.Top; y <= overlap.Bottom; y++)
        {
            // Calculate the pixel coordinates within the textures
            var texCoord1 = new Vector2(
                (float)(x - lhsBounds.Left) * texture1.Width / lhsBounds.Width,
                (float)((y - lhsBounds.Top) * texture1.Height) / lhsBounds.Height);
            var texCoord2 = new Vector2(
                (float)((x - rhsBounds.Left) * texture2.Width) / rhsBounds.Width,
                (float)((y - rhsBounds.Top) * texture2.Height) / rhsBounds.Height);

            // Check if the pixels at the current position are not transparent for both entities
            if (IsCollidableCoord(lhs, texCoord1) && IsCollidableCoord(rhs, texCoord2))
            {
                return true;
            }
        }
        
        return false;
    }

    public void Save()
    {
        foreach (var (textureName, collisionData) in _collisionData)
        {
            var relativePath = textureName.GetCollisionDataFilepath();
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            FileWriter.SaveCollisionData(fullPath, collisionData);
        }
    }

    public class CollisionCheckColumn
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