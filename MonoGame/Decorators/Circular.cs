using System;
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
        
        if (location == collisionLocation)
        {
            var collisionRhsDelta = collisionLocation - rhs.Bounds.Center.ToVector2();
        
            if (Math.Abs(collisionRhsDelta.X) > Math.Abs(collisionRhsDelta.Y))
            {
                collisionLocation.Y = rhs.Bounds.Center.Y;
            }
            else
            {
                collisionLocation.X = rhs.Bounds.Center.X;
            }
        }

        if (location == collisionLocation)
        {
            return _previousNormal;
        }
        
        var normal = Vector2.Normalize(collisionLocation - location);

        _previousNormal = normal;
        
        Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }
    
    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle _)
    {
        const float percent = 0.1f; // Penetration percentage to correct
        const float slop = 0.01f; // Allowable penetration
        
        while (CollisionData.Collides(Bounds, rhs.CollisionData, rhs.Bounds, out var overlap) && overlap is { IsEmpty: false })
        {
            var collisionCoordinate = overlap.Value.Center.ToVector2();

            var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
            var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);

            var lhsCorrection = Math.Max(overlap.Value.Height - slop, 0.0f) / (1 / Mass + 1 / rhs.Mass) * percent * lhsNormal;
            var rhsCorrection = Math.Max(overlap.Value.Height - slop, 0.0f) / (1 / rhs.Mass + 1 / Mass) * percent * rhsNormal;
            
            if (!IsStatic)
            {
                Position -= lhsCorrection / Mass;
            }

            if (!rhs.IsStatic)
            {
                rhs.Position += rhsCorrection / rhs.Mass;
            }
        }
    }
}