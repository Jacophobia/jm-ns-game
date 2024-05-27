using Microsoft.Xna.Framework;
using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public class Update : GeneralAction
{
    public override void Begin() { }

    public override void Apply(GameObject gameObject, GameTime gameTime)
    {
        gameObject.Update(gameTime);
    }

    public override void End() { }
}