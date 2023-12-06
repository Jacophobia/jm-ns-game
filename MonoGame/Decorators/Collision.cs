using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Collision : EntityDecorator
{
    public Collision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    private static bool AreMovingTowardsEachOther(ICollidable lhs, ICollidable rhs)
    {
        var overlap = Rectangle.Intersect(lhs.Bounds, rhs.Bounds);

        if (overlap is { IsEmpty: true })
            return false;

        var collisionLocation = overlap.Center;
        
        if (Math.Abs(overlap.Center.X - rhs.Bounds.Center.X) > Math.Abs(overlap.Center.Y - rhs.Bounds.Center.Y))
        {
            collisionLocation.Y = rhs.Bounds.Center.Y;
        }
        else
        {
            collisionLocation.X = rhs.Bounds.Center.X;
        }
        
        // Calculate position differences
        var deltaPosition = (collisionLocation - lhs.Bounds.Center).ToVector2();
        deltaPosition.Normalize();
        
        Debug.Assert(!float.IsNaN(deltaPosition.X) && !float.IsNaN(deltaPosition.Y));

        // Calculate velocity differences
        var deltaVelocity = rhs.Velocity - lhs.Velocity;
        deltaVelocity.Normalize();
        
        Debug.Assert(!float.IsNaN(deltaVelocity.X) && !float.IsNaN(deltaVelocity.Y));

        // Calculate the dot product
        var dotProduct = Vector2.Dot(deltaPosition, deltaVelocity);

        // If the dot product is negative, objects are moving towards each other
        return dotProduct < 0;
    }

    protected override bool IsCollidingWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        overlap = null;
        
        if (IsStatic && rhs.IsStatic)
        {
            return false;
        }
        
        return AreMovingTowardsEachOther(this, rhs) 
               && CollisionData.Collides(Bounds, rhs.CollisionData, rhs.Bounds, out overlap);
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle overlap)
    {
        Debug.Assert(!IsStatic || !rhs.IsStatic);
        Debug.Assert(rhs != null);
        Debug.Assert(overlap is not { IsEmpty: true });

        var initialMagnitude = (Velocity + rhs.Velocity).Length();

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;
        
        var collisionLocation = overlap.Center.ToVector2();

        // Calculate the normal (n) and tangential (t) direction vectors
        var lhsNormal = CalculateCollisionNormal(rhs, collisionLocation);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionLocation);
        
        // Decompose velocities into normal and tangential components
        var v1N = lhsVelocity.X * lhsNormal.X + lhsVelocity.Y * lhsNormal.Y; // Dot product
        var v1T = -lhsVelocity.X * lhsNormal.Y + lhsVelocity.Y * lhsNormal.X; // Perpendicular dot product

        if (IsStatic)
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
            var newV1N = -RestitutionCoefficient * v1N;

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
            var combinedRestitution = (RestitutionCoefficient + rhs.RestitutionCoefficient) / 2f;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - rhs.Mass) + 2f * rhs.Mass * v2N) /
                         (Mass + rhs.Mass);
            var newV2N = combinedRestitution * (v2N * (rhs.Mass - Mass) + 2f * Mass * v1N) /
                         (Mass + rhs.Mass);

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

        Velocity = lhsVelocity;
        rhs.Velocity = rhsVelocity;

        var overlapMass = overlap.Mass();

        var lhsRestitution = overlapMass / Mass;
        var rhsRestitution = overlapMass / rhs.Mass;

        Position += lhsNormal * lhsRestitution;
        rhs.Position += -rhsNormal * rhsRestitution;
    }
}