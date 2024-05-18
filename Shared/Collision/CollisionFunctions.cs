using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Shared.Extensions;
using Shared.Singletons;

namespace Shared.Collision;

public static class CollisionFunctions
{
    private static CollisionManager _collisionManager;
    
    public static bool IsThereACollision(ICollidable lhs, ICollidable rhs)
    {
        if (lhs.IsStatic && rhs.IsStatic)
        {
            return false;
        }
        
        var overlap = Rectangle.Intersect(lhs.Bounds, rhs.Bounds);
        
        if (overlap is { IsEmpty: true })
            return false;
        
        var collisionLocation = overlap.Center.ToVector2();
        
        _collisionManager ??= CollisionManager.GetInstance();

        return (AreMovingTowardsEachOther(lhs.Velocity, lhs.Bounds.Center.ToVector2(), Vector2.Zero, collisionLocation) 
                || AreMovingTowardsEachOther(rhs.Velocity, rhs.Bounds.Center.ToVector2(), Vector2.Zero, collisionLocation))
               && _collisionManager.Collides(lhs.CurrentTexture, lhs.Bounds, rhs.CurrentTexture, rhs.Bounds);
    }

    public static void HandleCollision(ICollidable lhs, ICollidable rhs)
    {
        Debug.Assert(!lhs.IsStatic || !rhs.IsStatic);
        Debug.Assert(rhs != null);

        var initialMagnitude = (lhs.Velocity + rhs.Velocity).Length();

        var lhsVelocity = lhs.Velocity;
        var rhsVelocity = rhs.Velocity;

        var overlap = Rectangle.Intersect(lhs.Bounds, rhs.Bounds);
        
        Debug.Assert(overlap is not { IsEmpty: true });

        // Calculate the normal (n) and tangential (t) direction vectors
        var (lhsNormal, rhsNormal) = CalculateCollisionNormals(lhs, rhs);
        rhsNormal *= -1;
        
        // Decompose velocities into normal and tangential components
        var v1N = lhsVelocity.X * lhsNormal.X + lhsVelocity.Y * lhsNormal.Y; // Dot product
        var v1T = -lhsVelocity.X * lhsNormal.Y + lhsVelocity.Y * lhsNormal.X; // Perpendicular dot product

        if (lhs.IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -rhs.RestitutionCoefficient *
                         (rhsVelocity.X * rhsNormal.X + rhsVelocity.Y * rhsNormal.Y);
            rhsVelocity.X = newV2N * rhsNormal.X - (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.Y;
            rhsVelocity.Y = newV2N * rhsNormal.Y + (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.X;
        }
        else if (rhs.IsStatic)
        {
            // Collision with a static object
            var newV1N = -lhs.RestitutionCoefficient * v1N;

            // Recompose velocity for the dynamic object
            lhsVelocity.X = newV1N * lhsNormal.X - v1T * lhsNormal.Y;
            lhsVelocity.Y = newV1N * lhsNormal.Y + v1T * lhsNormal.X;
        }
        else
        {
            // Collision with another dynamic object
            var v2N = rhsVelocity.X * rhsNormal.X + rhsVelocity.Y * rhsNormal.Y;
            var v2T = -rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X;

            // Apply the restitution coefficient
            var combinedRestitution = (lhs.RestitutionCoefficient + rhs.RestitutionCoefficient) / 2f;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (lhs.Mass - rhs.Mass) + 2f * rhs.Mass * v2N) /
                         (lhs.Mass + rhs.Mass);
            var newV2N = combinedRestitution * (v2N * (rhs.Mass - lhs.Mass) + 2f * lhs.Mass * v1N) /
                         (lhs.Mass + rhs.Mass);

            // Recompose velocities for both objects
            lhsVelocity.X = newV1N * lhsNormal.X - v1T * lhsNormal.Y;
            lhsVelocity.Y = newV1N * lhsNormal.Y + v1T * lhsNormal.X;
            rhsVelocity.X = newV2N * rhsNormal.X - v2T * rhsNormal.Y;
            rhsVelocity.Y = newV2N * rhsNormal.Y + v2T * rhsNormal.X;
        }

        var currentMagnitude = (lhsVelocity + rhsVelocity).Length();
        
        if (initialMagnitude != 0 && currentMagnitude >= initialMagnitude)
        {
            lhsVelocity *= initialMagnitude / currentMagnitude;
            rhsVelocity *= initialMagnitude / currentMagnitude;
        }

        lhs.Velocity = lhsVelocity;
        rhs.Velocity = rhsVelocity;

        var overlapMass = overlap.Mass();
        var lhsRestitution = overlapMass / lhs.Mass;
        var rhsRestitution = overlapMass / rhs.Mass;
        
        lhs.Position -= lhsNormal * lhsRestitution;
        rhs.Position += rhsNormal * rhsRestitution;
    }
    
    private static bool AreMovingTowardsEachOther(Vector2 lhsVelocity, Vector2 lhsPosition, Vector2 rhsVelocity, Vector2 rhsPosition)
    {
        // Calculate the relative position vector between the two objects
        var relativePosition = rhsPosition - lhsPosition;

        // Calculate the relative velocity vector between the two objects
        var relativeVelocity = rhsVelocity - lhsVelocity;

        // Calculate the dot product of the relative position and relative velocity vectors
        var dotProduct = Vector2.Dot(relativePosition, relativeVelocity);

        // If the dot product is negative, the objects are moving towards each other
        return dotProduct < 0;
    }

    private static Vector2 GetCollisionNormal(ICollidable lhs, ICollidable rhs)
    {
        Vector2 normal;
        var collisionLocation = Rectangle.Intersect(lhs.Bounds, rhs.Bounds).Center.ToVector2();
        
        switch (lhs.CollisionType)
        {
            case CollisionType.Circular:
                var location = lhs.Position;

                if (location == collisionLocation)
                {
                    location = lhs.PreviousPosition;
                }
                
                Debug.Assert(location != collisionLocation, "location cannot be equal to collisionLocation or it will wig out");

                normal = Vector2.Normalize(collisionLocation - location);
                
                Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
                break;
            case CollisionType.Rectangular:
                // Distance between the centers
                var distance = rhs.Position - lhs.Position;

                // Half extents along each axis
                var lhsHalfWidth = lhs.Bounds.Width / 2f;
                var lhsHalfHeight = lhs.Bounds.Height / 2f;
                var rhsHalfWidth = rhs.Bounds.Width / 2f;
                var rhsHalfHeight = rhs.Bounds.Height / 2f;

                // Calculate overlap on each axis
                var overlapX = lhsHalfWidth + rhsHalfWidth - MathF.Abs(distance.X);
                var overlapY = lhsHalfHeight + rhsHalfHeight - MathF.Abs(distance.Y);

                // The axis with the smallest overlap determines the normal
                normal = overlapX < overlapY ? new Vector2(MathF.Sign(distance.X), 0) : new Vector2(0, MathF.Sign(distance.Y));
        
                Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lhs.CollisionType), "Unsupported collision type");
        }

        return normal;
    }


    private static (Vector2, Vector2) CalculateCollisionNormals(ICollidable lhs, ICollidable rhs)
    {
        return (GetCollisionNormal(lhs, rhs), GetCollisionNormal(rhs, lhs));
    }
}