using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class Inertia : EntityDecorator
{
    public Inertia(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        if (IsStatic)
            return;
        var position = Position;
        
        Position += Velocity * deltaTime;
        
        // Debug.Assert(Position != position || Velocity == Vector2.Zero, "Position should have changed but did not");
    }
}