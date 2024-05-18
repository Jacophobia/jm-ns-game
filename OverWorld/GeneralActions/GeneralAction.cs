using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public abstract class GeneralAction
{
    public abstract void Begin();
    public abstract void Apply(GameObject gameObject, float deltaTime);
    public abstract void End();
}