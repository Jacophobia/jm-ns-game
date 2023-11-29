using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.File;

namespace MonoGame.Sprites;

internal class CollisionData
{
    // At some point, it would be good to make a custom data structure 
    // which could order points so that you can perform a more specific
    // search.
    private static IDictionary<string, List<List<CollisionCheckColumn>>> _collisionData;

    private readonly string _name;

    internal CollisionData(Texture2D texture)
    {
        _name = texture.Name;

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

    internal bool IsCollidableCoord(Point coordinate)
    {
        if (coordinate.X >= Data.Count)
            return false;
        
        var column = Data[coordinate.X];
        var count = 0;
        
        foreach (var check in column)
        {
            count += check.Count;
            if (count >= coordinate.Y) 
                return check.IsCollidable;
        }

        return false;
    }
    
    internal bool IsCollidableCoord(Vector2 coordinate)
    {
        coordinate.Round();
        return IsCollidableCoord(coordinate.ToPoint());
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
            return $"{IsCollidable},{Count}";
        }
    }
}