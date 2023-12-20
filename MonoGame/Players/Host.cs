using System;
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
    private static readonly Guid HostId = Guid.NewGuid();
    
    private readonly Renderer _renderer;

    public Host(Camera perspective, Renderer renderer) : base(HostId, perspective, new Listener(new Dictionary<Keys, Controls>
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

    protected override void OnDisplay(IWritable writable, SpriteFont font = null, string text = null, Vector2? position = null,
        Color? color = null, float? rotation = null, Vector2? origin = null, Vector2? scale = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null) 
    {
        _renderer.Write(writable, font, text, position, color, rotation, origin, scale, effect, depth);
    }

    public override void EndDisplay()
    {
        _renderer.End();
    }
}