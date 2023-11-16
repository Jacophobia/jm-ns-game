using System;
using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class Bound : EntityDecorator
{
    private Rectangle _bounds;

    public Bound(Entity @base, Rectangle bounds) : base(@base)
    {
        _bounds = bounds;
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
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
        var velocity = Velocity;

        if (Destination.X < _bounds.Left)
            velocity.X = MathF.Abs(velocity.X);
        if (Destination.X > _bounds.Right - Destination.Width)
            velocity.X = MathF.Abs(velocity.X) * -1;
        if (Destination.Y < _bounds.Top)
            velocity.Y = MathF.Abs(velocity.Y);
        if (Destination.Y > _bounds.Bottom - Destination.Height)
            velocity.Y = MathF.Abs(velocity.Y) * -1;

        Velocity = velocity;
    }
}