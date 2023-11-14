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

public class Elastic : EntityDecorator
{
    public Elastic(Entity @base) : base(@base)
    {
        // no new behavior to add
    }
    
    private static Vector2 CalculateDirection(Vector2 to, Vector2 from)
    {
        return Vector2.Normalize(to - from); // Normalizing to get a unit vector
    }

    private bool CollidesWith(ICollidable collidable)
    {
        return Sprite.Overlaps(Destination, collidable.Destination, out var newOverlap)
               && newOverlap.HasValue
               && Sprite.Collides(Sprite, Destination, collidable.Sprite, collidable.Destination,
                   newOverlap.Value,
                   out _);
    }
    
    private void Rewind(ICollidable collidable, GameTime gameTime)
    { 
        // Position -= Velocity * gameTime.DeltaTime();
        // collidable.Position -= collidable.Velocity * gameTime.DeltaTime();
        
        var direction = CalculateDirection(Position, collidable.Position);
        direction.Normalize();
        
        do
        {
            if (!IsStatic)
                Position += direction * Velocity.Length() * gameTime.DeltaTime();
            if (!collidable.IsStatic)
                collidable.Position -= direction * collidable.Velocity.Length() * gameTime.DeltaTime();
        } while (CollidesWith(collidable));
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        var velocity = Velocity;

        // Calculate the normal (n) and tangential (t) direction vectors
        var nx = collidable.Position.X - Position.X;
        var ny = collidable.Position.Y - Position.Y;
        var distance = MathF.Sqrt(nx * nx + ny * ny);
        nx /= distance; // Normalize
        ny /= distance; // Normalize

        // Decompose velocities into normal and tangential components
        var v1n = Velocity.X * nx + Velocity.Y * ny; // Dot product
        var v1t = -Velocity.X * ny + Velocity.Y * nx; // Perpendicular dot product
        var v2n = collidable.Velocity.X * nx + collidable.Velocity.Y * ny;
        var v2t = -collidable.Velocity.X * ny + collidable.Velocity.Y * nx;

        // Calculate new normal velocities considering mass
        float mass1 = Mass; // Assuming 'Mass' is a property of your entity
        float mass2 = collidable.Mass; // Similarly, assuming the collidable object has a 'Mass' property
        var newV1n = (v1n * (mass1 - mass2) + 2 * mass2 * v2n) / (mass1 + mass2);
        var newV2n = (v2n * (mass2 - mass1) + 2 * mass1 * v1n) / (mass1 + mass2);

        // Recompose velocities
        velocity.X = newV1n * nx - v1t * ny;
        velocity.Y = newV1n * ny + v1t * nx;
        collidable.Velocity = new Vector2(newV2n * nx - v2t * ny, newV2n * ny + v2t * nx);

        Velocity = velocity;

        Rewind(collidable, gameTime);
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