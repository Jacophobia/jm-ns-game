using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Friction : EntityDecorator
{
    private readonly float _frictionCoefficient;
    
    public Friction(Entity @base, float coefficient) : base(@base)
    {
        _frictionCoefficient = coefficient;
    }

    protected override void AfterHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap)
    {
        if (!rhs.IsStatic)
            return;
        
        Velocity -= Velocity * _frictionCoefficient * deltaTime;
    }
}