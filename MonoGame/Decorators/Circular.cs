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
        
        // Debug.Assert(_previousLocation != location, "The object did not move between frames");

        var normal = location != collisionLocation
            ? Vector2.Normalize(collisionLocation - location)
            : _previousNormal;
        _previousNormal = normal;
        
        Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }
    
    private static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2,
        Vector2 velocity2)
    {
        // Calculate position differences
        var deltaPosition = Vector2.Normalize(position2 - position1);

        // Calculate velocity differences
        var deltaVelocity = Vector2.Normalize(velocity2 - velocity1);

        // Calculate the dot product
        var dotProduct = Vector2.Dot(deltaPosition, deltaVelocity);

        // If the dot product is negative, objects are moving towards each other
        return dotProduct < 0;
    }
    
    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle overlap)
    {
        const float percent = 0.5f; // Penetration percentage to correct
        const float slop = 0.0001f; // Allowable penetration
        
        var collisionCoordinate = overlap.Center.ToVector2();
        
        var collisionRhsDelta = overlap.Center - rhs.Bounds.Center;
        
        var collisionLocation = collisionCoordinate;
        
        if (Math.Abs(collisionRhsDelta.X) > Math.Abs(collisionRhsDelta.Y))
        {
            collisionLocation.Y = rhs.Bounds.Center.Y;
        }
        else
        {
            collisionLocation.X = rhs.Bounds.Center.X;
        }

        var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);
        
        var lhsCorrection = Math.Max(overlap.Height - slop, 0.0f) / (1 / Mass + 1 / rhs.Mass) * percent * lhsNormal;
        var rhsCorrection = Math.Max(overlap.Height - slop, 0.0f) / (1 / rhs.Mass + 1 / Mass) * percent * rhsNormal;
        
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