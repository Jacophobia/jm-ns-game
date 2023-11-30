using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Inertia : EntityDecorator
{
    public Inertia(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(float deltaTime, IList<Controls> controls)
    {
        Position += Velocity * deltaTime;
    }
}