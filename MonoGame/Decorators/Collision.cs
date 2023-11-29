using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Collision : EntityDecorator
{
    public Collision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    private static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2,
        Vector2 velocity2)
    {
        // Calculate position differences
        var deltaPosition = position2 - position1;

        // Calculate velocity differences
        var deltaVelocity = velocity2 - velocity1;

        // Calculate the dot product using the static method from the math library
        var dotProduct = Vector2.Dot(deltaPosition, deltaVelocity);

        // If the dot product is negative, objects are moving towards each other
        return dotProduct < 0;
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        Debug.Assert(rhs != null);
        Debug.Assert(collisionLocation != null);
        Debug.Assert(overlap != null);

        var collisionCoordinate = overlap.Value.Center.ToVector2();
        // TODO: there is a tunneling issue where if two things intersect at the edge, this check will prevent a collision where there needs to be one
        if (!AreMovingTowardsEachOther(Destination.Center.ToVector2(), Velocity, collisionCoordinate, rhs.Velocity)) return;

        var initialMagnitude = (Velocity + rhs.Velocity).Length();

        if (IsStatic && rhs.IsStatic) return;

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;

        // Calculate the normal (n) and tangential (t) direction vectors
        var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);
        
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
        // TODO: To make it so that two types of collision can interact, the OnHandleCollisionFrom method should be called on the other object and it should be up to that object whether or not it moves.
        // TODO: We may also need to recalculate so that this method uses the RHS center coordinate which is closest to LHS 
    }
}