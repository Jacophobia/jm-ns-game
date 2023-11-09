using client.Entities;
using Microsoft.Xna.Framework;

namespace client.Decorators;

public class Gravity : EntityDecorator
{
    private readonly float _amount;
    
    public Gravity(Entity @base, float amount) : base(@base)
    {
        _amount = amount;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Origin = new Vector2(Origin.X, Origin.Y - _amount);
    }
}