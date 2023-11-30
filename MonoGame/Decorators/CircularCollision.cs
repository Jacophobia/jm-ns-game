using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class CircularCollision : EntityDecorator
{
    public CircularCollision(Entity @base) : base(@base)
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

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime,
        Rectangle? overlap)
    {
        Debug.Assert(rhs != null);
        Debug.Assert(overlap != null);

        // TODO: there is a tunneling issue where if two things intersect at the edge, this check will prevent a collision where there needs to be one
        if (!AreMovingTowardsEachOther(Position, Velocity, rhs.Position, rhs.Velocity)) return;

        var initialMagnitude = (Velocity + rhs.Velocity).Length();

        if (IsStatic && rhs.IsStatic) return;

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;

        // Calculate the normal (n) and tangential (t) direction vectors
        var n = rhs.Position - Position;
        n.Normalize();

        // Decompose velocities into normal and tangential components
        var v1N = lhsVelocity.X * n.X + lhsVelocity.Y * n.Y; // Dot product
        var v1T = -lhsVelocity.X * n.Y + lhsVelocity.Y * n.X; // Perpendicular dot product

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -rhs.RestitutionCoefficient *
                         (rhsVelocity.X * n.X + rhsVelocity.Y * n.Y);
            rhsVelocity.X = newV2N * n.X - (-rhsVelocity.X * n.Y + rhsVelocity.Y * n.X) * n.Y;
            rhsVelocity.Y = newV2N * n.Y + (-rhsVelocity.X * n.Y + rhsVelocity.Y * n.X) * n.X;
        }
        else if (rhs.IsStatic)
        {
            // Collision with a static object
            var newV1N = -RestitutionCoefficient * v1N;

            // Recompose velocity for the dynamic object
            lhsVelocity.X = newV1N * n.X - v1T * n.Y;
            lhsVelocity.Y = newV1N * n.Y + v1T * n.X;
        }
        else
        {
            // Collision with another dynamic object
            var v2N = rhsVelocity.X * n.X + rhsVelocity.Y * n.Y;
            var v2T = -rhsVelocity.X * n.Y + rhsVelocity.Y * n.X;

            // Apply the restitution coefficient
            var combinedRestitution = (RestitutionCoefficient + rhs.RestitutionCoefficient) / 2f;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - rhs.Mass) + 2f * rhs.Mass * v2N) /
                         (Mass + rhs.Mass);
            var newV2N = combinedRestitution * (v2N * (rhs.Mass - Mass) + 2f * Mass * v1N) /
                         (Mass + rhs.Mass);

            // Recompose velocities for both objects
            lhsVelocity.X = newV1N * n.X - v1T * n.Y;
            lhsVelocity.Y = newV1N * n.Y + v1T * n.X;
            rhsVelocity.X = newV2N * n.X - v2T * n.Y;
            rhsVelocity.Y = newV2N * n.Y + v2T * n.X;
        }

        const float nearlyOne = 0.99f;

        // TODO: update this so that it is adjusted in a less band-aid way. Basically right now there is a floating point error where the velocity increases slightly each hit, making things accelerate
        while (initialMagnitude != 0 && (lhsVelocity + rhsVelocity).Length() >= initialMagnitude)
        {
            lhsVelocity *= nearlyOne;
            rhsVelocity *= nearlyOne;
        }

        Velocity = lhsVelocity;
        rhs.Velocity = rhsVelocity;
        // TODO: To make it so that two types of collision can interact, the OnHandleCollisionFrom method should be called on the other object and it should be up to that object whether or not it moves.
        // TODO: We may also need to recalculate so that this method uses the RHS center coordinate which is closest to LHS 
    }
}