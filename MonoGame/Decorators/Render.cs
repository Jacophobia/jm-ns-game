using MonoGame.Entities;
using MonoGame.Players;

namespace MonoGame.Decorators;

public class Render : EntityDecorator
{
    public Render(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Player player)
    {
        player.Display(this);
    }
}