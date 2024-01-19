using Microsoft.Xna.Framework;
using MonoGame.Constants;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Jump : EntityDecorator
{
    private readonly IPlayer _player;
    private readonly float _jumpVelocity;
    private readonly float _hangTime;
    private readonly float _rechargeTime;
    private readonly float _stationaryThreshold;
    
    private float _jumpAcceleration;

    public Jump(Entity @base, IPlayer controlSource, float jumpVelocity, float hangTime, float rechargeTime, float stationaryThreshold) 
        : base(@base)
    {
        _player = controlSource;
        _jumpVelocity = jumpVelocity * Physics.PixelsToMeterRatio;
        _hangTime = hangTime;
        _rechargeTime = rechargeTime;
        _stationaryThreshold = stationaryThreshold;

        _jumpAcceleration = 0f;
    }

    private bool _wasJumpPressed;

    private void Impulse(float deltaTime, bool isJumping)
    {
        if (Velocity.Y > 0 && Velocity.Y < _stationaryThreshold)
        {
            _jumpAcceleration += _jumpVelocity * deltaTime / _rechargeTime;
            if (_jumpAcceleration > _jumpVelocity)
                _jumpAcceleration = _jumpVelocity;
        }

        if (!isJumping)
        {
            if (Velocity.Y < 0)
                Velocity = new Vector2(Velocity.X, 0);
            
            _wasJumpPressed = false;
            return;
        }
        
        _jumpAcceleration -= _jumpVelocity * deltaTime / _hangTime;

        if (_jumpAcceleration < 0f)
            _jumpAcceleration = 0f;
        
        if (!_wasJumpPressed && _jumpVelocity / _jumpAcceleration < 4f)
        {
            Velocity = new Vector2(Velocity.X, -_jumpAcceleration);
            _jumpAcceleration *= 0.9f;
        }

        _wasJumpPressed = true;
    }

    protected override void OnUpdate(float deltaTime)
    {
        var isJumping = (_player.Controls & Controls.Jump) != 0;
        
        Impulse(deltaTime, isJumping);
    }
}