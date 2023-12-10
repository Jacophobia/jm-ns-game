using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using MonoGame.Interfaces;

namespace MonoGame.Input;

internal class Listener : IControlSource
{
    private readonly IDictionary<Buttons, Controls> _controllerMapping;
    private readonly IDictionary<Keys, Controls> _keyboardMapping;

    internal Listener(IDictionary<Keys, Controls> mapping)
    {
        _keyboardMapping = mapping;
        _controllerMapping = new Dictionary<Buttons, Controls>();
    }

    internal Listener(IDictionary<Buttons, Controls> mapping)
    {
        _controllerMapping = mapping;
        _keyboardMapping = new Dictionary<Keys, Controls>();
    }

    internal Listener(IDictionary<Keys, Controls> keyboardMapping, IDictionary<Buttons, Controls> controllerMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    internal Listener(IDictionary<Buttons, Controls> controllerMapping, IDictionary<Keys, Controls> keyboardMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    internal void Swap(Keys lhs, Keys rhs)
    {
        (_keyboardMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _keyboardMapping[lhs]);
    }

    internal void Swap(Keys lhs, Buttons rhs)
    {
        (_keyboardMapping[lhs], _controllerMapping[rhs]) = (_controllerMapping[rhs], _keyboardMapping[lhs]);
    }

    internal void Swap(Buttons lhs, Keys rhs)
    {
        (_controllerMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _controllerMapping[lhs]);
    }

    internal void Swap(Buttons lhs, Buttons rhs)
    {
        (_controllerMapping[lhs], _controllerMapping[rhs]) = (_controllerMapping[rhs], _controllerMapping[lhs]);
    }

    public Controls GetControls(IPlayer player)
    {
        var keyboardState = Keyboard.GetState();
        var controls = _keyboardMapping.Keys
            .Where(key => keyboardState[key] == KeyState.Down)
            .Aggregate(Controls.None, (current, key) => current | _keyboardMapping[key]);

        if (player.Id.Key is not int playerIndex)
            return controls;
        
        var controllerState = GamePad.GetState(playerIndex);
        controls |= _controllerMapping.Keys
            .Where(button => controllerState.IsButtonDown(button))
            .Aggregate(Controls.None, (current, button) => current | _controllerMapping[button]);

        return controls;
    }
}