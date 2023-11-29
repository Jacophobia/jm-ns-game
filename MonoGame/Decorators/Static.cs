using Microsoft.Xna.Framework;
using MonoGame.Entities;

namespace MonoGame.Decorators;

public class Static : EntityDecorator
{
    public Static(Entity @base) : base(@base)
    {
        IsStatic = true;
        Velocity = Vector2.Zero;
    }
}