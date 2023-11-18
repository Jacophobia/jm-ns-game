using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class Render : EntityDecorator
{
    public Render(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera camera)
    {
        renderer.Render(this, camera);
    }
}