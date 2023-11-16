using System;
using System.Diagnostics;
using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class CircularCollision : EntityDecorator
{
    public CircularCollision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }
    
    public static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2, Vector2 velocity2)
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
        // TODO: To make it so that two types of collision can interact, the OnHandleCollisionFrom method should be called on the other object and it should be up to that object whether or not it moves.
        // TODO: We may also need to recalculate so that this method uses the RHS center coordinate which is closest to LHS 
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