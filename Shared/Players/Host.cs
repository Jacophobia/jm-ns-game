using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared.Input;
using Shared.Output;

namespace Shared.Players;

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

    protected override void OnDisplay(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth)
    {
        _renderer.Draw(texture, destination, source, color, rotation, origin, effect, depth);
    }

    public override void EndDisplay()
    {
        _renderer.End();
    }
}