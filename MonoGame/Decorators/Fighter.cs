using System.Collections.Generic;
using MonoGame.Attacks;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Fighter : EntityDecorator
{
    private readonly IPlayer _player;
    private readonly IDictionary<Controls, Attack> _attacks;
    private Attack _currentAttack;
    private Attack _attackBuffer;
    
    public Fighter(Entity @base, IPlayer player, IDictionary<Controls, Attack> attacks) : base(@base)
    {
        _player = player;
        _attacks = attacks;
        _currentAttack = null;
        _attackBuffer = null;
    }
    
    

    protected override void OnUpdate(float deltaTime)
    {
        if (_currentAttack?.Execute(this, deltaTime) ?? true)
        {
            _currentAttack = null;
        }

        var controls = _player.Controls;

        if (controls == Controls.None)
            return;

        if ((controls & Controls.ModifierOne) != 0)
        {
            
        }
    }
}