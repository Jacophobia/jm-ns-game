using System;
using System.Diagnostics;
using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using IO.Sprites;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class TestCollision : EntityDecorator
{
    public TestCollision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }
    
    private void Rewind(ICollidable collidable, GameTime gameTime)
    { 
        Position -= Velocity * gameTime.DeltaTime();
        collidable.Position -= collidable.Velocity * gameTime.DeltaTime();
    }
    
    private static Vector2 CalculateDirection(Vector2 to, Vector2 from)
    {
        return Vector2.Normalize(to - from); // Normalizing to get a unit vector
    }
    
    private void AdjustVelocity(ICollidable collidable)
    {
        var velocity = Velocity;
        
        if (IsStatic)
        {
            return;
        }

        // Calculate the normal (n) and tangential (t) direction vectors
        var nx = collidable.Position.X - Position.X;
        var ny = collidable.Position.Y - Position.Y;
        var distance = MathF.Sqrt(nx * nx + ny * ny);
        nx /= distance; // Normalize
        ny /= distance; // Normalize

        // Decompose velocities into normal and tangential components
        var v1N = Velocity.X * nx + Velocity.Y * ny; // Dot product
        var v1T = -Velocity.X * ny + Velocity.Y * nx; // Perpendicular dot product
        
        // Collision with another dynamic object
        var v2N = collidable.Velocity.X * nx + collidable.Velocity.Y * ny;

        // Apply the restitution coefficient
        var combinedRestitution = (RestitutionCoefficient + collidable.RestitutionCoefficient) / 2;

        // Exchange normal components in an inelastic collision
        var newV1N = combinedRestitution * (v1N * (Mass - collidable.Mass) + 2 * collidable.Mass * v2N) /
                     (Mass + collidable.Mass);

        // Recompose velocities for both objects
        velocity.X = newV1N * nx - v1T * ny;
        velocity.Y = newV1N * ny + v1T * nx;

        Velocity = velocity;
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        AdjustVelocity(collidable);
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        var tries = 0;
        
        var direction = CalculateDirection(Position, collidable.Position);
        direction.Normalize();
        
        do
        {

            if (!IsStatic)
                Position += direction;
            if (!collidable.IsStatic)
                collidable.Position -= direction;

            tries++;
        } while (Sprite.Overlaps(Destination, collidable.Destination, out var newOverlap)
                 && newOverlap.HasValue
                 && Sprite.Collides(Sprite, Destination, collidable.Sprite, collidable.Destination, 
                     newOverlap.Value,
                     out _));
        
        AdjustVelocity(collidable);
        collidable.HandleCollisionFrom(this, gameTime, collisionLocation, overlap);
    }

    protected override void OnDraw(Camera camera)
    {
        // no new behavior to add
    }
}