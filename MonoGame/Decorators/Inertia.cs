using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Inertia : EntityDecorator
{
    public Inertia(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(float deltaTime)
    {
        Position += Velocity * deltaTime;
    }
}