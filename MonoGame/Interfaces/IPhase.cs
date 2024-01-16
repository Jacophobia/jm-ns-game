using MonoGame.Entities;

namespace MonoGame.Interfaces;

public interface IPhase
{
    public void Execute(Entity context);
}