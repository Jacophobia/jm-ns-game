using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Gravity : EntityDecorator
{
    private const float GravitationalAcceleration = -9.8f;
    
    public Gravity(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, IList<Controls> controls)
    {
        Velocity = new Vector2(Velocity.X, Velocity.Y + GravitationalAcceleration * gameTime.DeltaTime());
    }
}