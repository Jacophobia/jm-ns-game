using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class BasicCollisionHandler : EntityDecorator
{
    public BasicCollisionHandler(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Camera camera)
    {
        // no new behavior to add
    }
}