using Microsoft.Xna.Framework;
using MonoGame.Constants;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class LRMovement : EntityDecorator
{
    private readonly IPlayer _player;
    private readonly float _runningVelocity;
    
    public LRMovement(Entity @base, IPlayer player, float runningVelocity) : base(@base)
    {
        _player = player;
        _runningVelocity = runningVelocity * Physics.PixelsToMeterRatio;
    }

    protected override void OnUpdate(float deltaTime)
    {
        var left = (_player.Controls & Controls.Left) != 0;
        var right = (_player.Controls & Controls.Right) != 0;
        
        var velocity = new Vector2(0, Velocity.Y);

        if (left)
        {
            velocity.X -= _runningVelocity;
        }

        if (right)
        {
            velocity.X += _runningVelocity;
        }

        Velocity = velocity;
    }
}