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
        
        return CollisionData.Collides(lhsBounds, rhs.CollisionData, rhsBounds, out overlap);
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

        var lhsRestitution = Math.Min(2 * overlapMass / Mass, RestitutionCoefficient);
        var rhsRestitution = Math.Min(2 * overlapMass / rhs.Mass, rhs.RestitutionCoefficient);

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
