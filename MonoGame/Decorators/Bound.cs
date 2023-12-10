using System;
using Microsoft.Xna.Framework;
using MonoGame.Entities;

namespace MonoGame.Decorators;

public class Bound : EntityDecorator
{
    private Rectangle _bounds;

    public Bound(Entity @base, Rectangle bounds) : base(@base)
    {
        _bounds = bounds;
    }

    protected override void OnUpdate(float deltaTime)
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