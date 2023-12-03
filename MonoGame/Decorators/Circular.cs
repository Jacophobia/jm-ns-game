using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Players;

namespace MonoGame.Decorators;

public class Circular : EntityDecorator
{
    private Vector2 _previousLocation;
    
    public Circular(Entity @base) : base(@base)
    {
        Debug.Assert(Velocity.X != 0f && Velocity.Y != 0f, "Velocity should not be zero initially (This assertion is just to test something. Remove it if it gets in the way)");
        _previousLocation = Destination.Center.ToVector2() - Velocity;
        Debug.Assert(Destination.Center.ToVector2() != _previousLocation, "Velocity should not be zero initially (This assertion is just to test something. Remove it if it gets in the way)");
    }

    protected override Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        var location = Destination.Center.ToVector2();
        
        // Debug.Assert(_previousLocation != location, "The object did not move between frames");

        var normal = location != collisionLocation
            ? Vector2.Normalize(collisionLocation - location)
            : Vector2.Normalize(collisionLocation - _previousLocation);
        
        // Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }

    private bool first = true;
    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        if (first)
        {
            first = false;
            return;
        }
        
        var location = Destination.Center.ToVector2();
        
        Debug.Assert(_previousLocation != location, "The object did not move between frames");

        _previousLocation = location;
    }
}