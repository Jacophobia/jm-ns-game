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
        var myDestination = Destination;
        var otherDestination = collidable.Destination;

        myDestination.X -= (int)MathF.Ceiling(Velocity.X);
        myDestination.Y -= (int)MathF.Ceiling(Velocity.Y);

        otherDestination.X -= (int)MathF.Ceiling(collidable.Velocity.X);
        otherDestination.Y -= (int)MathF.Ceiling(collidable.Velocity.Y);

        Destination = myDestination;
        collidable.Destination = otherDestination;
    }

    public override void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap)
    {
        RewindOneFrame(collidable);

        var color = Color;
        var velocity = Velocity;
        
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        
        color.R++;
        color.G++;
        color.B++;
        // (_color.R, _color.G, _color.B) = (Color.G, Color.B, Color.R);

        // Calculate the normal (n) and tangential (t) direction vectors
        float nx = collidable.Destination.Center.X - Destination.Center.X;
        float ny = collidable.Destination.Center.Y - Destination.Center.Y;
        var distance = MathF.Sqrt(nx * nx + ny * ny);
        nx /= distance; // Normalize
        ny /= distance; // Normalize

        // Decompose velocities into normal and tangential components
        var v1n = Velocity.X * nx + Velocity.Y * ny; // Dot product
        var v1t = -Velocity.X * ny + Velocity.Y * nx; // Perpendicular dot product
        var v2n = collidable.Velocity.X * nx + collidable.Velocity.Y * ny;
        var v2t = -collidable.Velocity.X * ny + collidable.Velocity.Y * nx;

        // Exchange normal components in elastic collision
        (v1n, v2n) = (v2n, v1n);

        // Recompose velocities
        velocity.X = v1n * nx - v1t * ny;
        velocity.Y = v1n * ny + v1t * nx;
        collidable.Velocity = new Vector2(v2n * nx - v2t * ny, v2n * ny + v2t * nx);

        Color = color;
        Velocity = velocity;
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