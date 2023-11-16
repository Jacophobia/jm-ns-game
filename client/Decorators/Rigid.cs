using System;
using System.Diagnostics;
using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class Rigid : EntityDecorator
{
    public Rigid(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    public void AttemptOne(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");
        // Calculate collision normal
        var collisionNormal = Vector2.Normalize(collidable.Position - Position);
        
        // Calculate relative velocity
        var relativeVelocity = collidable.Velocity - Velocity;
        var velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);
        
        // Early exit if velocities are separating
        if (velocityAlongNormal > 0)
            return;
        
        // Use the minimum restitution coefficient
        var restitution = Math.Min(RestitutionCoefficient, collidable.RestitutionCoefficient);
        
        // Calculate impulse scalar
        var impulseScalar = -(1f + restitution) * velocityAlongNormal;
        
        // Adjust calculations if one of the objects is static
        if (IsStatic)
        {
            impulseScalar /= (1f / collidable.Mass);
        }
        else if (collidable.IsStatic)
        {
            impulseScalar /= (1f / Mass);
        }
        else
        {
            impulseScalar /= (1f / Mass) + (1f / collidable.Mass);
        }
        
        // Calculate and apply impulse
        var impulse = impulseScalar * collisionNormal;
        if (!IsStatic)
        {
            Velocity += (impulse / Mass);
        }
        if (!collidable.IsStatic)
        {
            collidable.Velocity -= (impulse / collidable.Mass);
        }
    }
    
    protected void AttemptTwo(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collidable != null);
        Debug.Assert(collisionLocation != null);
        Debug.Assert(overlap != null);
        
        var totalVelocity = Velocity + collidable.Velocity;
        
        var collisionNormal = Vector2.Normalize(collidable.Position - Position);
        
        // Calculate relative velocity
        var relativeVelocity = collidable.Velocity - Velocity;
        var velocityAlongNormal = Vector2.Dot(relativeVelocity, collisionNormal);
        
        // Early exit if velocities are separating
        if (velocityAlongNormal > 0)
            return;

        // Calculate the restitution coefficient
        var restitution = Math.Min(RestitutionCoefficient, collidable.RestitutionCoefficient);

        // Calculate new velocities
        var newVelocityThisLength = CalculateNewVelocity(Velocity, Mass, collidable.Velocity, collidable.Mass, restitution);
        var newVelocityOtherLength = CalculateNewVelocity(collidable.Velocity, collidable.Mass, Velocity, Mass, restitution);

        // Update velocities
        Velocity += collisionNormal * newVelocityThisLength;
        collidable.Velocity -= collisionNormal * newVelocityOtherLength;

        var newTotalVelocity = Velocity + collidable.Velocity;

        var totalMagnitude = totalVelocity.Length();
        var newTotalMagnitude = newTotalVelocity.Length();
        
        Debug.Assert(Math.Abs(newTotalVelocity.Length()) <= Math.Abs(totalVelocity.Length()));
    }
    
    private static float CalculateNewVelocity(Vector2 velocity1, float mass1, Vector2 velocity2, float mass2, float restitution)
    {
        return (mass1 * velocity1.Length() + mass2 * velocity2.Length() + mass2 * restitution * (velocity2.Length() - velocity1.Length())) / (mass1 + mass2);
    }
    
    public static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2, Vector2 velocity2)
    {

        // var distance = Math.Abs((collidable.Position - Position).Length());
        //
        // var lhsNextPosition = Position + Velocity;
        // var rhsNextPosition = collidable.Position + collidable.Velocity;
        //
        // var futureDistance = Math.Abs((rhsNextPosition - lhsNextPosition).Length());
        //
        // // Early exit if velocities are separating
        // if (futureDistance > distance)
        //     return;
        // Calculate position differences
        var deltaPosition = position2 - position1;

        // Calculate velocity differences
        var deltaVelocity = velocity2 - velocity1;

        // Calculate the dot product using the static method from the math library
        var dotProduct = Vector2.Dot(deltaPosition, deltaVelocity);

        // If the dot product is negative, objects are moving towards each other
        return dotProduct < 0;
    }

    protected void AttemptThree(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // AttemptOne(collidable, gameTime, collisionLocation, overlap);
        // return;
        Debug.Assert(collidable != null);
        Debug.Assert(collisionLocation != null);
        Debug.Assert(overlap != null);

        if (!AreMovingTowardsEachOther(Position, Velocity, collidable.Position, collidable.Velocity))
            return;
        
        var collisionNormal = Vector2.Normalize(collidable.Position - Position);

        var initialMagnitude = Velocity.Length();

        // Calculate the restitution coefficient
        var restitution = Math.Min(RestitutionCoefficient, collidable.RestitutionCoefficient);

        // Calculate new velocities
        var newVelocityThisLength = CalculateNewVelocity(Velocity, Mass, collidable.Velocity, collidable.Mass, restitution);
        var newVelocityOtherLength = CalculateNewVelocity(collidable.Velocity, collidable.Mass, Velocity, Mass, restitution);

        if (!IsStatic)
        {
            Velocity += collisionNormal * (newVelocityThisLength - Velocity.Length());
        }
        if (!collidable.IsStatic)
        {
            collidable.Velocity += collisionNormal * (newVelocityOtherLength - collidable.Velocity.Length());
        }

        const float nearlyOne = 0.999999f;

        while (Velocity.Length() >= initialMagnitude)
            Velocity *= nearlyOne;
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        Debug.Assert(collidable != null);
        Debug.Assert(collisionLocation != null);
        Debug.Assert(overlap != null);

        if (!AreMovingTowardsEachOther(Position, Velocity, collidable.Position, collidable.Velocity))
            return;
        
        var initialMagnitude = Velocity.Length();
        
        if (IsStatic && collidable.IsStatic) return;

        var velocity = Velocity;
        var otherVelocity = collidable.Velocity;

        // Calculate the normal (n) and tangential (t) direction vectors
        var n = collidable.Position - Position;
        n.Normalize();

        // Decompose velocities into normal and tangential components
        var v1N = Velocity.X * n.X + Velocity.Y * n.Y; // Dot product
        var v1T = -Velocity.X * n.Y + Velocity.Y * n.X; // Perpendicular dot product

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -collidable.RestitutionCoefficient *
                         (collidable.Velocity.X * n.X + collidable.Velocity.Y * n.Y);
            otherVelocity.X = newV2N * n.X - (-collidable.Velocity.X * n.Y + collidable.Velocity.Y * n.X) * n.Y;
            otherVelocity.Y = newV2N * n.Y + (-collidable.Velocity.X * n.Y + collidable.Velocity.Y * n.X) * n.X;
        }
        else if (collidable.IsStatic)
        {
            // Collision with a static object
            var newV1N = -RestitutionCoefficient * v1N;

            // Recompose velocity for the dynamic object
            velocity.X = newV1N * n.X - v1T * n.Y;
            velocity.Y = newV1N * n.Y + v1T * n.X;
        }
        else
        {
            // Collision with another dynamic object
            var v2N = collidable.Velocity.X * n.X + collidable.Velocity.Y * n.Y;
            var v2T = -collidable.Velocity.X * n.Y + collidable.Velocity.Y * n.X;

            // Apply the restitution coefficient
            var combinedRestitution = (RestitutionCoefficient + collidable.RestitutionCoefficient) / 2f;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - collidable.Mass) + 2f * collidable.Mass * v2N) /
                         (Mass + collidable.Mass);
            var newV2N = combinedRestitution * (v2N * (collidable.Mass - Mass) + 2f * Mass * v1N) /
                         (Mass + collidable.Mass);

            // Recompose velocities for both objects
            velocity.X = newV1N * n.X - v1T * n.Y;
            velocity.Y = newV1N * n.Y + v1T * n.X;
            otherVelocity.X = newV2N * n.X - v2T * n.Y;
            otherVelocity.Y = newV2N * n.Y + v2T * n.X;
        }

        const float nearlyOne = 0.999999f;

        while (velocity.Length() >= initialMagnitude)
            velocity *= nearlyOne;

        Velocity = velocity;
        collidable.Velocity = otherVelocity;
    }



    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera camera)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }
}