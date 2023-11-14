using System;
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

    private void RewindOneFrame(ICollidable collidable)
    {
        Position -= Velocity;
        collidable.Position -= collidable.Velocity;
    }

    public override void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        RewindOneFrame(collidable);

        var velocity = Velocity;
        var otherVelocity = collidable.Velocity;

        
        // Skip processing if both objects are static
        if (IsStatic && collidable.IsStatic)
        {
            return;
        }

        // Calculate the normal (n) and tangential (t) direction vectors
        float nx = collidable.Destination.Center.X - Destination.Center.X;
        float ny = collidable.Destination.Center.Y - Destination.Center.Y;
        var distance = MathF.Sqrt(nx * nx + ny * ny);
        nx /= distance; // Normalize
        ny /= distance; // Normalize

        // Decompose velocities into normal and tangential components
        var v1N = Velocity.X * nx + Velocity.Y * ny; // Dot product
        var v1T = -Velocity.X * ny + Velocity.Y * nx; // Perpendicular dot product

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -collidable.RestitutionCoefficient * (collidable.Velocity.X * nx + collidable.Velocity.Y * ny);
            otherVelocity.X = newV2N * nx - (-collidable.Velocity.X * ny + collidable.Velocity.Y * nx) * ny;
            otherVelocity.Y = newV2N * ny + (-collidable.Velocity.X * ny + collidable.Velocity.Y * nx) * nx;
        }
        else if (collidable.IsStatic)
        {
            // Collision with a static object
            var newV1N = -RestitutionCoefficient * v1N;

            // Recompose velocity for the dynamic object
            velocity.X = newV1N * nx - v1T * ny;
            velocity.Y = newV1N * ny + v1T * nx;
        }
        else
        {
            // Collision with another dynamic object
            var v2N = collidable.Velocity.X * nx + collidable.Velocity.Y * ny;
            var v2T = -collidable.Velocity.X * ny + collidable.Velocity.Y * nx;

            // Apply the restitution coefficient
            var combinedRestitution = (RestitutionCoefficient + collidable.RestitutionCoefficient) / 2;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - collidable.Mass) + 2 * collidable.Mass * v2N) /
                         (Mass + collidable.Mass);
            var newV2N = combinedRestitution * (v2N * (collidable.Mass - Mass) + 2 * Mass * v1N) /
                         (Mass + collidable.Mass);

            // Recompose velocities for both objects
            velocity.X = newV1N * nx - v1T * ny;
            velocity.Y = newV1N * ny + v1T * nx;
            otherVelocity.X = newV2N * nx - v2T * ny;
            otherVelocity.Y = newV2N * ny + v2T * nx;
        }
        
        // Positional correction
        var correctionDirection = CalculateDirection(Destination.Center.ToVector2(), collidable.Destination.Center.ToVector2());
        var penetrationDepth = MathF.Min(overlap.Value.Width, overlap.Value.Height);
        const float positionalCorrectionFactor = 0.4f; // Adjust this factor as needed

        // Calculate inverse mass (0 for static objects)
        float inverseMassThis = IsStatic ? 0 : 1 / Mass;
        float inverseMassOther = collidable.IsStatic ? 0 : 1 / collidable.Mass;
        var totalInverseMass = inverseMassThis + inverseMassOther;

        // Skip correction if total inverse mass is zero (both objects are static)
        if (totalInverseMass > 0)
        {
            // The amount each object is moved is proportional to its inverse mass
            var correctionAmount = correctionDirection * penetrationDepth * positionalCorrectionFactor / totalInverseMass;

            if (!IsStatic)
            {
                Position += correctionAmount * inverseMassThis;
            }

            if (!collidable.IsStatic)
            {
                collidable.Position -= correctionAmount * inverseMassOther;
            }
        }

        Velocity = velocity;
        collidable.Velocity = otherVelocity;
    }

    public override void Draw(Camera camera)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }
}