using MonoGame.Entities;
using MonoGame.Interfaces;
using MonoGame.Players;

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