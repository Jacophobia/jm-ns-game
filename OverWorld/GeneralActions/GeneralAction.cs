using Microsoft.Xna.Framework;
using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public abstract class GeneralAction
{
    public abstract void Begin();
    public abstract void Apply(GameObject gameObject, GameTime gameTime);
    public abstract void End();
}