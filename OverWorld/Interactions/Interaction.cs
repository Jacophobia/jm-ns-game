using OverWorld.GameObjects;

namespace OverWorld.Interactions;

public abstract class Interaction
{
    public abstract void Apply(IGameObject lhs, IGameObject rhs);
}