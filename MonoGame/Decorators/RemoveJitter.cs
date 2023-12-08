using System;
using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class RemoveJitter : EntityDecorator
{
    private readonly float _minimumVelocity;
    
    public RemoveJitter(Entity @base, float minimumVelocity) : base(@base)
    {
        _minimumVelocity = minimumVelocity;
    }

    protected override void BeforeUpdate(float deltaTime)
    {
        var velocity = Velocity;
        if (Math.Abs(Velocity.X) < _minimumVelocity) 
            velocity.X = 0f;
        if (Math.Abs(Velocity.Y) < _minimumVelocity)
            velocity.Y = 0f;
        Velocity = velocity;
    }
}