using client.Entities;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class RectangularCollision : EntityDecorator
{
    public RectangularCollision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }

    private static Vector2 CalculateDirection(Vector2 to, Vector2 from)
    {
        return Vector2.Normalize(to - from); // Normalizing to get a unit vector
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        var lhs = this;

        var checks = 0;

        if (lhs.Destination.Left < rhs.Destination.Left) checks++;

        if (lhs.Destination.Right > rhs.Destination.Right) checks++;

        if (lhs.Destination.Top < rhs.Destination.Top) checks++;

        if (lhs.Destination.Bottom > rhs.Destination.Bottom) checks++;

        if (checks >= 4)
        {
        }
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