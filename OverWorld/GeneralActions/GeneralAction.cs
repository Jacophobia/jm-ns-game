using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public abstract class GeneralAction
{
    public abstract void Apply(IGameObject gameObject, float deltaTime);
}