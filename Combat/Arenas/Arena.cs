using System.Collections.Generic;
using Combat.Decorations;
using Combat.Fighters;
using Combat.Training;
using Shared.Collision;
using Shared.Players;

namespace Combat.Arenas;

public abstract class Arena
{
    private CombatLog _log;

    // fighters
    private Fighter _fighterOne;
    private Fighter _fighterTwo;
    
    // battle area
    private readonly ArenaType _type;
    private readonly List<Floor> _floorPanels;
    private Wall _leftWall;
    private Wall _rightWall;
    private List<Decoration> _decorations;

    protected Arena(ArenaType type, List<Decoration> decorations)
    {
        _type = type;
        _decorations = decorations;
        // if the arena type is enclosed, add walls
        throw new System.NotImplementedException();
    }

    public void BeginFight(Fighter fighterOne, Fighter fighterTwo)
    {
        throw new System.NotImplementedException();
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
        
        foreach (var panel in _floorPanels)
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

    public void Update(float deltaTime)
    {
        // floor only needs to be updated if it is ArenaType.Infinite
        throw new System.NotImplementedException();
    }

    public void Render(IPlayer player)
    {
        // draw the fighters, floor, walls (if applicable), and decorations
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns true if the fight has ended.
    /// </summary>
    /// <param name="winner">
    /// Will be set to the winner if the fight has ended. Will be null otherwise.
    /// </param>
    /// <returns>Whether or not the fight has concluded</returns>
    public bool TryGetWinner(out Fighter winner)
    {
        throw new System.NotImplementedException();
    }
}