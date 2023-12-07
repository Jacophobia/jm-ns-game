using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Host : Player
{
    private readonly Renderer _renderer;

    public Host(Camera perspective, Renderer renderer) : base(perspective, new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down },
            { Keys.X, Controls.Jump }
        }))
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