using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Xna.Framework.Input;
using Shared.Configuration;
using Shared.Controllables;
using Shared.GameControllers;
using Shared.Rendering;
using Shared.View;
using TestArena;

var builder = Host.CreateApplicationBuilder(args);

// transient (game)
builder.Services.AddTransient<GameController, ArenaGameController>();

// scoped
// TODO: Load this from a config file
PlayerController Controller(IServiceProvider _)
{
    var controls = new Dictionary<Keys, Controls>
    {
        { Keys.A, Controls.Left },
        { Keys.E, Controls.Right },
        { Keys.OemComma, Controls.Up },
        { Keys.O, Controls.Down },
        { Keys.X, Controls.Jump }
    };
    return new PlayerController(controls);
}

// TODO: Load this from a config file
GameSettings Settings(IServiceProvider _)
{
    return new GameSettings
    {
        Fullscreen = false,
    };
}

builder.Services.AddScoped(Settings);
builder.Services.AddScoped<IController, PlayerController>(Controller);
builder.Services.AddScoped<MonogameRenderPipeline>();
builder.Services.AddScoped<Camera>();
builder.Services.AddScoped<LocalRenderer>();

// singleton (rendering, collision detector, players)

using var host = builder.Build();
using var scope = host.Services.CreateScope();
using var game = scope.ServiceProvider.GetService<GameController>();

game?.Run();
