using client.Entities;
using Microsoft.Xna.Framework;

namespace client.Decorators;

public class Gravity : EntityDecorator
{
    public Gravity(Entity @base) : base(@base)
    {
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Velocity = new Vector2(Velocity.X, Velocity.Y - 9.8f);
    }
}