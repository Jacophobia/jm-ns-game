using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public class Update : GeneralAction
{
    public override void Begin() { }

    public override void Apply(GameObject gameObject, float deltaTime)
    {
        gameObject.Update(deltaTime);
    }

    public override void End() { }
}