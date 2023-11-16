using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class Drag : EntityDecorator
{
    private readonly float _rate;

    public Drag(Entity @base, float rate) : base(@base)
    {
        _rate = rate;
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        Velocity -= Velocity * _rate * gameTime.DeltaTime();
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        Velocity -= Velocity * _rate * 2 * gameTime.DeltaTime();
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
}