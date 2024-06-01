using System;
using System.Collections.Generic;
using Combat.Decorations;
using Combat.Fighters;
using Combat.Training;
using Microsoft.Xna.Framework;
using Shared.Collision;
using Shared.Rendering;
using Shared.Updates;
using Shared.View;

namespace Combat.Arenas;

public class Arena : IRenderable, IUpdatable
{
    private CombatLog _log;
    private bool _isGameOver;
    private static readonly TimeSpan FullCelebrationDuration = TimeSpan.FromSeconds(2);
    private TimeSpan _celebrationDuration;

    // fighters
    private Fighter _fighterOne;
    private Fighter _fighterTwo;
    
    // battle area
    private readonly Floor _floor;
    private readonly Wall _leftWall;
    private readonly Wall _rightWall;
    
    // decorations / effects
    private readonly List<Decoration> _decorations;

    public Arena(ArenaType type, List<Decoration> decorations)
    {
        _decorations = decorations;
        
        _floor = new Floor(type == ArenaType.Infinte);
        
        // if the arena type is enclosed, add walls
        if (type == ArenaType.Enclosed)
        {
            // add walls
            // _leftWall = new Wall(right: _floor.Left - 1);
            // _rightWall = new Wall(left: _floor.Right + 1);
            throw new System.NotImplementedException();
        }
    }

    public void BeginFight(Fighter fighterOne, Fighter fighterTwo)
    {
        _celebrationDuration = FullCelebrationDuration;
        _fighterOne = fighterOne;
        _fighterTwo = fighterTwo;
    }

    private static void HandleCollision(ICollidable lhs, ICollidable rhs)
    {
        if (CollisionFunctions.IsThereACollision(lhs, rhs))
            CollisionFunctions.HandleCollision(lhs, rhs);
    }

    private void HandleCollisions(ICollidable fighter)
    {
        if (_leftWall != null && _rightWall != null)
        {
            HandleCollision(fighter, _leftWall);
            HandleCollision(fighter, _rightWall);
        }
        
        foreach (var panel in _floor)
        {
            HandleCollision(fighter, panel);
        }
    }

    private void HandleCollisions()
    {
        HandleCollision(_fighterOne, _fighterTwo);
        HandleCollisions(_fighterOne);
        HandleCollisions(_fighterTwo);
    }

    public void Update(GameTime gameTime)
    {
        _fighterOne.Update(gameTime);
        _fighterTwo.Update(gameTime);
        _leftWall?.Update(gameTime);
        _rightWall?.Update(gameTime);
        _floor.Update(gameTime, _fighterOne, _fighterTwo);
        
        HandleCollisions();
    }

    public void Render(IRenderer renderer, Camera camera)
    {
        _fighterOne.Render(renderer, camera);
        _fighterTwo.Render(renderer, camera);
        _leftWall?.Render(renderer, camera);
        _rightWall?.Render(renderer, camera);

        foreach (var floorPanel in _floor)
        {
            floorPanel.Render(renderer, camera);
        }

        foreach (var decoration in _decorations)
        {
            decoration.Render(renderer, camera);
        }
    }

    /// <summary>
    /// Returns true if the fight has ended.
    /// </summary>
    /// <param name="gameTime">
    /// Elapsed time since last frame.
    /// </param>
    /// <param name="winner">
    /// Will be set to the winner if the fight has ended. Will be null otherwise.
    /// </param>
    /// <returns>Whether or not the fight has concluded</returns>
    public bool TryGetWinner(GameTime gameTime, out Fighter winner)
    {
        if (_fighterOne.Health <= 0)
        {
            winner = _fighterTwo;
        }
        else if (_fighterTwo.Health <= 0)
        {
            winner = _fighterOne;
        }
        else
        {
            winner = null;
            return false;
        }

        _celebrationDuration -= gameTime.ElapsedGameTime;

        return _celebrationDuration <= TimeSpan.Zero;
    }
}