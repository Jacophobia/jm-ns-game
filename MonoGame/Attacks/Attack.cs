using System.Collections.Generic;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Attacks;

public class Attack
{
    private readonly List<IPhase> _phases;

    public Attack()
    {
        _phases = new List<IPhase>();
    }

    public Attack Then(IPhase next)
    {
        _phases.Add(next);
        return this;
    }

    public void Execute(Entity context)
    {
        foreach (var phase in _phases)
        {
            phase.Execute(context);
        }
    }
}