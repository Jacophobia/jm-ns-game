using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Drag : EntityDecorator
{
    private readonly float _dragCoefficient;
    
    public Drag(Entity @base, float coefficient) : base(@base)
    {
        _dragCoefficient = coefficient;
    }

    protected override void OnUpdate(float deltaTime)
    {
        Velocity -= Velocity * _dragCoefficient * deltaTime;
    }
}