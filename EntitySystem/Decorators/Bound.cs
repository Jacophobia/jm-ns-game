using System;
using Collision.Interfaces;
using EntitySystem.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;

namespace EntitySystem.Decorators;

public class Bound : EntityDecorator
{
    private Rectangle _bounds;

    public Bound(Entity @base, Rectangle bounds) : base(@base)
    {
        _bounds = bounds;
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
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

        if (Destination.X < _bounds.Left + Destination.Width / 2)
            velocity.X = MathF.Abs(velocity.X);
        if (Destination.X > _bounds.Right - Destination.Width / 2)
            velocity.X = MathF.Abs(velocity.X) * -1;
        if (Destination.Y < _bounds.Top + Destination.Height / 2)
            velocity.Y = MathF.Abs(velocity.Y);
        if (Destination.Y > _bounds.Bottom - Destination.Height / 2)
            velocity.Y = MathF.Abs(velocity.Y) * -1;

        Velocity = velocity;
    }
}