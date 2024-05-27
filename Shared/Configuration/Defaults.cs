using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Shared.Controllables;

namespace Shared.Configuration;

public static class Defaults
{
    public static Dictionary<Keys, Controls> Controls => new()
    {
        { Keys.A, Controllables.Controls.Left },
        { Keys.E, Controllables.Controls.Right },
        { Keys.OemComma, Controllables.Controls.Up },
        { Keys.O, Controllables.Controls.Down },
        { Keys.X, Controllables.Controls.Jump }
    };
}