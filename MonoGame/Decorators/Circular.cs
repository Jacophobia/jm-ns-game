using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Circular : EntityDecorator
{
    public Circular(Entity @base) : base(@base)
    {
    }

    protected override Vector2 OnCalculateCollisionNormal(Entity rhs, Vector2 collisionLocation)
    {
        var location = Bounds.Center.ToVector2();

        if (location == collisionLocation)
            location = PreviousBounds.Center.ToVector2();

        var normal = Vector2.Normalize(collisionLocation - location);
        Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }
}