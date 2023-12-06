using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Circular : EntityDecorator
{
    private Vector2 _previousNormal;
    
    public Circular(Entity @base) : base(@base)
    {
        Debug.Assert(Velocity.X != 0f && Velocity.Y != 0f, "Velocity should not be zero initially (This assertion is just to test something. Remove it if it gets in the way)");
        _previousNormal = Vector2.Normalize(Destination.Center.ToVector2() - Velocity);
    }

    protected override Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        var location = Destination.Center.ToVector2();

        var normal = location != collisionLocation
            ? Vector2.Normalize(collisionLocation - location)
            : _previousNormal;
        
        _previousNormal = normal;
        
        Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }
}