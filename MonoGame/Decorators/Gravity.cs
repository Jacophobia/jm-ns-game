using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Decorators;

public class Gravity : EntityDecorator
{
    public Gravity(Entity @base) : base(@base)
    {
        // no new behavior to add
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
        Velocity = new Vector2(Velocity.X, Velocity.Y - 9.8f * gameTime.DeltaTime());
    }
}