using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Remote : IPlayer
{
    private const int LocalPlayerId = 0;
    
    private readonly Renderer _renderer;
    private readonly NetworkClient _networkClient;
    private readonly Listener _listener;

    public Remote(Renderer renderer, Rectangle perspective, NetworkClient networkClient, float depth = -10, float focalLength = Camera.FocalLength)
    {
        _renderer = renderer;
        Perspective = perspective;
        Depth = depth;
        FocalLength = focalLength;
        _networkClient = networkClient;
        Id = new PlayerId(LocalPlayerId);
        _listener = new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down },
            { Keys.X, Controls.Jump } // TODO: find out why jumping currently is broken for the remote client
        });
    }

    public PlayerId Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; private set; }

    public void Follow(IRenderable target)
    {
        // currently we don't have a need for this since the client camera doesn't move. It should not be called
        throw new System.NotImplementedException(); 
    }

    public void BeginDisplay()
    { 
        _renderer.Begin();
    }

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null,
        Color? color = null, float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None,
        float? depth = null)
    {
        _renderer.Draw(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Display(IWritable writable)
    {
        _renderer.Write(writable);
    }

    public void Update(float deltaTime)
    {
        Controls = _listener.GetControls(this);
        
        _networkClient.SendControlData(Controls);
    }

    public void EndDisplay()
    {
        _renderer.End();
    }
}