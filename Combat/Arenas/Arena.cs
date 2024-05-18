using System.Collections.Generic;
using Combat.Decorations;
using Combat.Fighters;
using Combat.Training;
using MonoGame.Interfaces;

namespace Combat.Arenas;

public abstract class Arena
{
    private readonly ArenaType _type;
    private CombatLog _log;

    private Fighter _fighterOne;
    private Fighter _fighterTwo;

    private List<Floor> _floorPanels;
    
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

    public void Update(float deltaTime)
    {
        // floor only needs to be updated if it is ArenaType.Infinite
        throw new System.NotImplementedException();
    }

    public void Render(IPlayer player)
    {
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