using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class SeparateOnCollision : EntityDecorator
{
    public SeparateOnCollision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap)
    {
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        if (IsStatic && rhs.IsStatic)
            return;

        const int maxTries = 10;
        var tries = 0;
        while (tries < maxTries)
        {
            if (!IsStatic)
                Position -= Velocity * deltaTime;
            if (!rhs.IsStatic)
                rhs.Position -= rhs.Velocity * deltaTime;

            if (CollidesWith(rhs, deltaTime, out _))
                tries = maxTries;

            tries++;
        }
    }
}