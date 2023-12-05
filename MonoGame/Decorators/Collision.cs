using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Collision : EntityDecorator
{
    public Collision(Entity @base) : base(@base)
    {
        // No new behavior to add
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

    protected override bool IsCollidingWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        if (IsStatic && rhs.IsStatic)
        {
            overlap = null;
            return false;
        }

        var lhsBounds = Bounds;
        var rhsBounds = rhs.Bounds;

        lhsBounds.Location += (Velocity * deltaTime).ToPoint();
        rhsBounds.Location += (rhs.Velocity * deltaTime).ToPoint();
        
        // Find the intersection rectangle
        overlap = Rectangle.Intersect(lhsBounds, rhsBounds);

        // TODO: there is an issue where if something gets completely enveloped into something else between frames, it freezes the game
        // TODO: we also need to incorporate the Source rectangle in this so that collisions are calculated correctly when a sprite sheet is used
        var collisionRhsDelta = overlap.Value.Center - rhsBounds.Center;
        
        var collisionLocation = overlap.Value.Center.ToVector2();
        
        if (Math.Abs(collisionRhsDelta.X) > Math.Abs(collisionRhsDelta.Y))
        {
            collisionLocation.Y = rhsBounds.Center.Y;
        }
        else
        {
            collisionLocation.X = rhsBounds.Center.X;
        }
        
        return overlap is not { IsEmpty: true }
                // && AreMovingTowardsEachOther(lhsBounds.Center.ToVector2(), Velocity, collisionLocation, rhs.Velocity)
                && CollisionData.Collides(lhsBounds, rhs.CollisionData, rhsBounds, overlap.Value);
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle overlap)
    {
        Debug.Assert(!IsStatic);
        Debug.Assert(!IsStatic || !rhs.IsStatic);
        Debug.Assert(rhs != null);
        Debug.Assert(overlap is not { IsEmpty: true });
        
        var initialMagnitude = (Velocity + rhs.Velocity).Length();

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;

        var collisionCoordinate = overlap.Center.ToVector2();
        
        var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);
        
        var v1N = Vector2.Dot(lhsVelocity, lhsNormal); // Dot product
        var v1T = -lhsVelocity.X * lhsNormal.Y + lhsVelocity.Y * lhsNormal.X; // Perpendicular dot product

        var overlapMass = overlap.Mass();

        var lhsRestitution = Math.Min(overlapMass / Mass, RestitutionCoefficient);
        var rhsRestitution = Math.Min(overlapMass / rhs.Mass, rhs.RestitutionCoefficient);

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -rhsRestitution *
                         (rhsVelocity.X * rhsNormal.X + rhsVelocity.Y * rhsNormal.Y);
            rhsVelocity.X = newV2N * rhsNormal.X - (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.Y;
            rhsVelocity.Y = newV2N * rhsNormal.Y + (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.X;
        }
        else if (rhs.IsStatic)
        {
            // Collision with a static object
            var newV1N = -lhsRestitution * v1N;

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
            var combinedRestitution = (lhsRestitution + rhsRestitution) / 2f;

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
    }
}
