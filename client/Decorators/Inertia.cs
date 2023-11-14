using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class Inertia : EntityDecorator
{
    public Inertia(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    public override void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    public override void Draw(Camera camera)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        
    }
}