using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Circular : EntityDecorator
{
    public Circular(Entity @base) : base(@base)
    {
    }

    protected override Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        return Vector2.Normalize(collisionLocation - Destination.Center.ToVector2());
    }
}