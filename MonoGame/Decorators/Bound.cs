using System;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Bound : EntityDecorator
{
    private Rectangle _bounds;

    public Bound(Entity @base, Rectangle bounds) : base(@base)
    {
        _bounds = bounds;
    }

    protected override void OnUpdate(float deltaTime, Controls controls)
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