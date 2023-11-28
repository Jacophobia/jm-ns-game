using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Sprites;

namespace MonoGame.Interfaces;

public interface ICollidable : IRenderable
{
    public Sprite Sprite { get; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float RestitutionCoefficient { get; set; }
    public bool IsStatic { get; set; }
    public float Mass { get; }

    public Rectangle GetPath(float deltaTime)
    {
        var endPosition = Position + Velocity * deltaTime;
        // Calculate the min and max points
        var minX = Math.Min(Position.X, endPosition.X);
        var minY = Math.Min(Position.Y, endPosition.Y);
        var maxX = Math.Max(Position.X, endPosition.X);
        var maxY = Math.Max(Position.Y, endPosition.Y);

        // Adjust the points to account for the dimensions of the entity
        minX -= Destination.Width / 2f;
        minY -= Destination.Height / 2f;
        maxX += Destination.Width / 2f;
        maxY += Destination.Height / 2f;

        // Create and return the path rectangle
        return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
    }

    public void Update(GameTime gameTime, IList<Controls> controls);

    public void HandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public void HandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public bool CollidesWith(ICollidable rhs, out Vector2? collisionLocation, out Rectangle? overlap)
    {
        if (Sprite.Overlaps(Destination, rhs.Destination, out overlap) && overlap.HasValue)
            return Sprite.Collides(Sprite, Destination, rhs.Sprite, rhs.Destination, overlap.Value,
                out collisionLocation);

        collisionLocation = null;
        return false;
    }
}