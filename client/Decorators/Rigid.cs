﻿using System;
using System.Diagnostics;
using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class Rigid : EntityDecorator
{
    public Rigid(Entity @base) : base(@base)
    {
        // no new behavior to add
    }
    
    private static Vector2 CalculateDirection(Vector2 to, Vector2 from)
    {
        return Vector2.Normalize(to - from); // Normalizing to get a unit vector
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        // Rewind(collidable);

        var velocity = Velocity;
        var otherVelocity = collidable.Velocity;
        
        // Skip processing if both objects are static
        if (IsStatic && collidable.IsStatic)
        {
            return;
        }

        // Calculate the normal (n) and tangential (t) direction vectors
        var n = collidable.Position - Position;
        n.Normalize();

        // Decompose velocities into normal and tangential components
        var v1N = Velocity.X * n.X + Velocity.Y * n.Y; // Dot product
        var v1T = -Velocity.X * n.Y + Velocity.Y * n.X; // Perpendicular dot product

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -collidable.RestitutionCoefficient * (collidable.Velocity.X * n.X + collidable.Velocity.Y * n.Y);
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
            var combinedRestitution = (RestitutionCoefficient + collidable.RestitutionCoefficient) / 2;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - collidable.Mass) + 2 * collidable.Mass * v2N) /
                         (Mass + collidable.Mass);
            var newV2N = combinedRestitution * (v2N * (collidable.Mass - Mass) + 2 * Mass * v1N) /
                         (Mass + collidable.Mass);

            // Recompose velocities for both objects
            velocity.X = newV1N * n.X - v1T * n.Y;
            velocity.Y = newV1N * n.Y + v1T * n.X;
            otherVelocity.X = newV2N * n.X - v2T * n.Y;
            otherVelocity.Y = newV2N * n.Y + v2T * n.X;
        }
        
        // Positional correction
        var correctionDirection = CalculateDirection(Position, collidable.Position);
        var otherCorrectionDirection = CalculateDirection(collidable.Position, Position);
        var penetrationDepth = MathF.Sqrt(overlap.Value.Width * overlap.Value.Width + overlap.Value.Height * overlap.Value.Height);
        const float positionalCorrectionFactor = 0.1f; // Adjust this factor as needed
        
        // Calculate inverse mass (0 for static objects)
        var inverseMassThis = IsStatic ? 0f : 1f / Mass;
        var inverseMassOther = collidable.IsStatic ? 0f : 1f / collidable.Mass;
        var totalInverseMass = inverseMassThis + inverseMassOther;
        var correction = correctionDirection * penetrationDepth * positionalCorrectionFactor / totalInverseMass * inverseMassThis;
        var otherCorrection = otherCorrectionDirection * penetrationDepth * positionalCorrectionFactor / totalInverseMass * inverseMassOther;
        
        if (!IsStatic)
        {
            Position += correction;
        }
    
        if (!collidable.IsStatic)
        {
            collidable.Position += otherCorrection;
        }

        Velocity = velocity;
        collidable.Velocity = otherVelocity;
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Camera camera)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }
}