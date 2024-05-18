using OverWorld.GameObjects;

namespace OverWorld.Interactions;

public abstract class Interaction
{
    public abstract void Apply(GameObject lhs, GameObject rhs);
}