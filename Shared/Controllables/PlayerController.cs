using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Shared.Controllables;

// ReSharper disable once InvertIf
public class PlayerController : IController
{
    private readonly IDictionary<Buttons, Controls> _controllerMapping;
    private readonly IDictionary<Keys, Controls> _keyboardMapping;

    public PlayerController(IDictionary<Keys, Controls> mapping)
    {
        _keyboardMapping = mapping;
        _controllerMapping = null;
    }

    public PlayerController(IDictionary<Buttons, Controls> mapping)
    {
        _controllerMapping = mapping;
        _keyboardMapping = null;
    }

    public PlayerController(IDictionary<Buttons, Controls> controllerMapping, IDictionary<Keys, Controls> keyboardMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    public void Swap(Keys lhs, Keys rhs)
    {
        (_keyboardMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _keyboardMapping[lhs]);
    }

    public void Swap(Buttons lhs, Buttons rhs)
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