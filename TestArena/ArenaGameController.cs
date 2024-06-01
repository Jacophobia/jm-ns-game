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
    private readonly Camera _camera;
    private readonly Fighter _player;
    private readonly Fighter _opponent;
    private readonly Arena _arena;

    public ArenaGameController(GameSettings settings) : base(settings)
    {
        _arena = new Arena(ArenaType.Infinte, new List<Decoration>());
        
        var playerController = new PlayerController(settings);
        _player = new Fighter(playerController, 100, 100);

        var opponentController = new DoNothingController();
        _opponent = new Fighter(opponentController, 100, 50);
        
        _camera = new Camera(_player);
    }

    protected override void OnLoad()
    {
        _arena.BeginFight(_player, _opponent);
    }

    protected override void OnPlayerConnected(Guid newPlayerId, IController controller)
    {
        
    }

    protected override void OnPlayerDisconnected(Guid newPlayerId)
    {
        
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (_arena.TryGetWinner(gameTime, out var winner))
        {
            Console.WriteLine(winner == _player ? "You win!" : "You lose!");
            Exit();
        }
        
        _arena.Update(gameTime);
        
        _camera.Update(gameTime);
    }

    protected override void OnDraw(GameTime gameTime, Dictionary<Guid, IRenderer> renderers)
    {
        foreach (var (_, renderer) in renderers)
        {
            _arena.Render(renderer, _camera);
        }
    }
}