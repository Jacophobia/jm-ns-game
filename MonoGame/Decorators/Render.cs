using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Render : EntityDecorator
{
    public Render(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnDraw(IPlayer player)
    {
        player.Display(this);
    }
}