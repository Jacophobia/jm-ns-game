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

    private static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2, Vector2 velocity2)
    {
        var deltaPosition = position2 - position1;
        var deltaVelocity = velocity2 - velocity1;
        return Vector2.Dot(deltaPosition, deltaVelocity) < 0;
    }

    protected override bool IsCollidingWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        overlap = null;

        if (IsStatic && rhs.IsStatic)
        {
            return false;
        }

        var relativeVelocity = Velocity - rhs.Velocity;

        if (GetCollisionTimes(Bounds, rhs.Bounds, relativeVelocity, deltaTime, out float enterTime, out float exitTime, out Vector2 normal))
        {
            var collisionPointLhs = Base.Position + Velocity * enterTime;
            var collisionPointRhs = rhs.Position + rhs.Velocity * enterTime;

            var lhsBoundsAtCollision = new Rectangle(collisionPointLhs.ToPoint(), Bounds.Size);
            var rhsBoundsAtCollision = new Rectangle(collisionPointRhs.ToPoint(), rhs.Bounds.Size);

            overlap = Rectangle.Intersect(lhsBoundsAtCollision, rhsBoundsAtCollision);

            return overlap != null && !overlap.IsEmpty && CollisionData.Collides(lhsBoundsAtCollision, rhs.CollisionData, rhsBoundsAtCollision, overlap);
        }

        return false;
    }

    private bool GetCollisionTimes(Rectangle lhsBounds, Rectangle rhsBounds, Vector2 velocity, float deltaTime, out float enterTime, out float exitTime, out Vector2 normal)
    {
        // Get times for X and Y axes
        var (enterX, exitX) = GetCollisionTimes1D(lhsBounds.X, rhsBounds.X, lhsBounds.Width, rhsBounds.Width, velocity.X, 0, deltaTime);
        var (enterY, exitY) = GetCollisionTimes1D(lhsBounds.Y, rhsBounds.Y, lhsBounds.Height, rhsBounds.Height, velocity.Y, 0, deltaTime);

        // Find the earliest/latest times of collision/leaving
        enterTime = Math.Max(enterX, enterY);
        exitTime = Math.Min(exitX, exitY);

        // Determine the collision normal
        normal = enterX > enterY ? new Vector2(velocity.X < 0 ? 1 : -1, 0) : new Vector2(0, velocity.Y < 0 ? 1 : -1);

        return enterTime < exitTime && enterTime >= 0 && enterTime < deltaTime;
    }

    private (float enter, float exit) GetCollisionTimes1D(float posA, float posB, float sizeA, float sizeB, float velA, float velB, float deltaTime)
    {
        float enterTime, exitTime;

        if (velA - velB == 0) // Parallel movement or static
        {
            enterTime = float.NegativeInfinity;
            exitTime = float.PositiveInfinity;
        }
        else
        {
            var relativePos = posB - posA;
            enterTime = (relativePos) / (velA - velB);
            exitTime = (relativePos + sizeB - sizeA) / (velA - velB);
        }

        if (enterTime > exitTime)
        {
            (enterTime, exitTime) = (exitTime, enterTime); // Swap if needed
        }

        return (enterTime, exitTime);
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap)
    {
        Debug.Assert(!IsStatic || !rhs.IsStatic);
        Debug.Assert(rhs != null);
        Debug.Assert(overlap != null);

        var collisionCoordinate = overlap.Value.Center.ToVector2();
        var collisionNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var inverseCollisionNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);

        var relativeVelocity = Velocity - rhs.Velocity;
        var velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);

        if (velocityAlongNormal > 0) return;

        var e = Math.Min(RestitutionCoefficient, rhs.RestitutionCoefficient);
        var j = -(1 + e) * velocityAlongNormal;
        j /= 1 / Mass + 1 / rhs.Mass;

        var impulse = j * collisionNormal;
        
        const float percent = 0.2f; // Penetration percentage to correct
        const float slop = 0.01f; // Allowable penetration
        var correction = Math.Max(overlap.Value.Height - slop, 0.0f) / (1 / Mass + 1 / rhs.Mass) * percent * collisionNormal;

        if (!IsStatic)
        {
            Position -= correction / Mass; // TODO: Fix the spatial partition so it allows us to separate the objects on collision
            Velocity -= impulse / Mass;
        }
        if (!rhs.IsStatic)
        {
            rhs.Position += correction / rhs.Mass;
            rhs.Velocity += impulse / rhs.Mass;
        }
    }
}
