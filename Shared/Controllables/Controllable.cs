using Microsoft.Xna.Framework;

namespace Shared.Controllables;

public abstract class Controllable
{
    private readonly IController _controller;
    
    protected Controllable(IController controller)
    {
        _controller = controller;
    }

    public void Update(GameTime gameTime)
    {
        OnUpdate(gameTime, _controller.Controls);
    }

    protected abstract void OnUpdate(GameTime gameTime, Controls controls);
}