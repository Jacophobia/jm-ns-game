using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Shared.Input;

namespace Shared.Controllers;

// ReSharper disable once InvertIf
internal class PlayerController : IController
{
    private readonly IDictionary<Buttons, Controls> _controllerMapping;
    private readonly IDictionary<Keys, Controls> _keyboardMapping;

    internal PlayerController(IDictionary<Keys, Controls> mapping)
    {
        _keyboardMapping = mapping;
        _controllerMapping = null;
    }

    internal PlayerController(IDictionary<Buttons, Controls> mapping)
    {
        _controllerMapping = mapping;
        _keyboardMapping = null;
    }

    internal PlayerController(IDictionary<Buttons, Controls> controllerMapping, IDictionary<Keys, Controls> keyboardMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    internal void Swap(Keys lhs, Keys rhs)
    {
        (_keyboardMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _keyboardMapping[lhs]);
    }

    internal void Swap(Buttons lhs, Buttons rhs)
    {
        (_controllerMapping[lhs], _controllerMapping[rhs]) = (_controllerMapping[rhs], _controllerMapping[lhs]);
    }

    public Controls Controls
    {
        get
        {
            var controls = Controls.None;
            
            if (_keyboardMapping != null)
            {
                var keyboardState = Keyboard.GetState();
                controls = _keyboardMapping.Keys
                    .Where(key => keyboardState[key] == KeyState.Down)
                    .Aggregate(Controls.None, (current, key) => current | _keyboardMapping[key]);
            }
            
            if (_controllerMapping != null)
            {
                const int playerIndex = 0;
                var controllerState = GamePad.GetState(playerIndex);
                controls |= _controllerMapping.Keys
                    .Where(button => controllerState.IsButtonDown(button))
                    .Aggregate(Controls.None, (current, button) => current | _controllerMapping[button]);
            }

            return controls;
        }
    }
}