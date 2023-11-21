using Collision.Interfaces;
using EntitySystem.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;

namespace EntitySystem.Decorators;

public class Static : EntityDecorator
{
    public Static(Entity @base) : base(@base)
    {
        IsStatic = true;
        Velocity = Vector2.Zero;
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
        // no new behavior to add
    }
}