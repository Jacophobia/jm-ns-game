using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class BasicMovement : EntityDecorator
{
    private const int JumpPower = 50;

    private readonly IPlayer _player;
    private Controls _previous;
    private int _holdCount;

    public BasicMovement(Entity @base, IPlayer player) : base(@base)
    {
        _player = player;
        _previous = Controls.None;
        _holdCount = 0;
    }

    protected override void OnUpdate(float deltaTime)
    {
        var velocity = Velocity;
        if ((_player.Controls & Controls.Jump) > (_previous & Controls.Jump) && Velocity.Y == 0)
            velocity.Y -= JumpPower - _holdCount++;
        else if ((_player.Controls & Controls.Jump & _previous) != 0 && _holdCount < JumpPower)
            velocity.Y -= JumpPower - _holdCount++;
        else if ((_player.Controls & Controls.Jump) < (_previous & Controls.Jump))
            _holdCount = 0;
        if ((_player.Controls & Controls.Down) != 0)
            velocity.Y += 500 * deltaTime;
        if ((_player.Controls & Controls.Left) != 0)
            velocity.X = -400;
        if ((_player.Controls & Controls.Right) != 0)
            velocity.X = 400;
        Velocity = velocity;
        _previous = _player.Controls;
    }
}