using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Shared.Configuration;
using Shared.Controllables;
using TestArena;

var settings = new GameSettings
{
    Fullscreen = false,
    Controls = new Dictionary<Keys, Controls>
    {
        { Keys.A, Controls.Left },
        { Keys.E, Controls.Right },
        { Keys.OemComma, Controls.Up },
        { Keys.O, Controls.Down },
        { Keys.X, Controls.Jump }
    },
    HostIp = "127.0.0.1",
    HostPort = 1234,
};

using var game = new ArenaGameController(settings);

game.Run();
