using MonoGame.Entities;
using MonoGame.Output;

namespace MonoGame.Decorators;

public class Render : EntityDecorator
{
    public Render(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera cameras)
    {
        renderer.Render(this, cameras);
    }
}