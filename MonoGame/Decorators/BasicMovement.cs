using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

public class BasicMovement : EntityDecorator
{
    private const int JumpPower = 50;
    
    private Controls _previous;
    private int _holdCount;
    
    public BasicMovement(Entity @base) : base(@base)
    {
        _previous = Controls.None;
        _holdCount = 0;
    }

    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        var velocity = Velocity;
        if ((controls & Controls.Jump) > (_previous & Controls.Jump) && Velocity.Y == 0)
            velocity.Y -= JumpPower - _holdCount++;
        else if ((controls & Controls.Jump & _previous) != 0 && _holdCount < JumpPower)
            velocity.Y -= JumpPower - _holdCount++;
        else if ((controls & Controls.Jump) < (_previous & Controls.Jump))
            _holdCount = 0;
        if ((controls & Controls.Down) != 0)
            velocity.Y += 500 * deltaTime;
        if ((controls & Controls.Left) != 0)
            velocity.X = -400;
        if ((controls & Controls.Right) != 0)
            velocity.X = 400;
        Velocity = velocity;
        _previous = controls;
    }
}