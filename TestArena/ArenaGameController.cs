using System;
using System.Collections.Generic;
using Combat.Arenas;
using Combat.Decorations;
using Combat.Fighters;
using Microsoft.Xna.Framework;
using Shared.Configuration;
using Shared.Controllables;
using Shared.GameControllers;
using Shared.Rendering;
using Shared.View;

namespace TestArena;

public class ArenaGameController : GameController
{
    private readonly Dictionary<Guid, Camera> _cameras;
    private readonly Queue<Fighter> _fighters;
    private readonly Arena _arena;

    public ArenaGameController(MonogameRenderPipeline renderPipeline, GameSettings settings)
        : base(renderPipeline, settings)
    {
        _cameras = new Dictionary<Guid, Camera>();
        _fighters = new Queue<Fighter>();
        _arena = new Arena(ArenaType.Infinte, new List<Decoration>());
        
        var controller = new PlayerController(settings.Controls);
        var fighter = new Fighter(controller, 100, 100);
        _fighters.Enqueue(fighter);
        
        var camera = new Camera(fighter);
        _cameras.Add(Guid.Empty, camera);
    }

    protected override void OnLoad()
    {
        
    }

    protected override void OnPlayerConnected(Guid newPlayerId, IController controller)
    {
        var fighter = new Fighter(controller, 100, 100);
        _fighters.Enqueue(fighter);

        var camera = new Camera(fighter);
        _cameras.Add(newPlayerId, camera);
    }

    protected override void OnPlayerDisconnected(Guid newPlayerId)
    {
        _cameras.Remove(newPlayerId);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _arena.Update(gameTime);
        
        foreach (var (_, camera) in _cameras)
        {
            camera.Update(gameTime);
        }
    }

    protected override void OnDraw(GameTime gameTime, Dictionary<Guid, IRenderer> renderers)
    {
        foreach (var (playerId, renderer) in renderers)
        {
            _arena.Render(renderer, _cameras[playerId]);
        }
    }
}