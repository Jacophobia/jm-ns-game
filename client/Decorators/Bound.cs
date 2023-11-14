using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class Bound : EntityDecorator
{
    private Rectangle _bounds;
    
    public Bound(Entity @base, Rectangle bounds) : base(@base)
    {
        _bounds = bounds;
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
        var velocity = Velocity;
        
        if ((Destination.Left < _bounds.Left && velocity.X < 0) || (Destination.Right > _bounds.Right && velocity.X > 0))
            velocity.X = 0f;
        if ((Destination.Bottom < _bounds.Bottom && velocity.Y < 0) || (Destination.Top > _bounds.Top && velocity.Y > 0))
            velocity.Y = 0f;

        Velocity = velocity;
    }
}