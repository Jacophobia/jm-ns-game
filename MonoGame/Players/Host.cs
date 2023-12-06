using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Host : Player
{
    private readonly Renderer _renderer;

    public Host(Camera perspective, Renderer renderer) : base(perspective)
    {
        _renderer = renderer;
    }

    public override void BeginDisplay()
    {
        _renderer.Begin();
    }

    protected override void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null)
    {
        _renderer.Draw(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public override void EndDisplay()
    {
        _renderer.End();
    }
}