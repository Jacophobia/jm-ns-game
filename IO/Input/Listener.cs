using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace IO.Input;

public class Listener
{
    private readonly IDictionary<Buttons, Controls> _controllerMapping;
    private readonly IDictionary<Keys, Controls> _keyboardMapping;

    public Listener(IDictionary<Keys, Controls> mapping)
    {
        _keyboardMapping = mapping;
        _controllerMapping = new Dictionary<Buttons, Controls>();
    }

    public Listener(IDictionary<Buttons, Controls> mapping)
    {
        _controllerMapping = mapping;
        _keyboardMapping = new Dictionary<Keys, Controls>();
    }

    public Listener(IDictionary<Keys, Controls> keyboardMapping, IDictionary<Buttons, Controls> controllerMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    public Listener(IDictionary<Buttons, Controls> controllerMapping, IDictionary<Keys, Controls> keyboardMapping)
    {
        _keyboardMapping = keyboardMapping;
        _controllerMapping = controllerMapping;
    }

    public void Swap(Keys lhs, Keys rhs)
    {
        (_keyboardMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _keyboardMapping[lhs]);
    }

    public void Swap(Keys lhs, Buttons rhs)
    {
        (_keyboardMapping[lhs], _controllerMapping[rhs]) = (_controllerMapping[rhs], _keyboardMapping[lhs]);
    }

    public void Swap(Buttons lhs, Keys rhs)
    {
        (_controllerMapping[lhs], _keyboardMapping[rhs]) = (_keyboardMapping[rhs], _controllerMapping[lhs]);
    }

    public void Swap(Buttons lhs, Buttons rhs)
    {
        (_controllerMapping[lhs], _controllerMapping[rhs]) = (_controllerMapping[rhs], _controllerMapping[lhs]);
    }

    public Controls GetInputState()
    {
        var keyboardState = Keyboard.GetState();
        var controllerState = GamePad.GetState(0);

        var controls = _keyboardMapping.Keys
            .Where(key => keyboardState[key] == KeyState.Up)
            .Aggregate(Controls.None, (current, key) => current | _keyboardMapping[key]);

        controls |= _controllerMapping.Keys
            .Where(button => controllerState.IsButtonDown(button))
            .Aggregate(Controls.None, (current, button) => current | _controllerMapping[button]);

        return controls;
    }
}