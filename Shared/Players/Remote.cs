using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared.Input;
using Shared.Networking;
using Shared.Output;

namespace Shared.Players;

public class Remote : IPlayer
{
    private static readonly Guid LocalPlayerId = Guid.NewGuid();
    
    private readonly Renderer _renderer;
    private readonly Client _client;
    private readonly Listener _listener;

    public Remote(Renderer renderer, Rectangle perspective, Client client, float depth = -10, float focalLength = Camera.FocalLength)
    {
        _renderer = renderer;
        Perspective = perspective;
        Depth = depth;
        FocalLength = focalLength;
        _client = client;
        Id = LocalPlayerId;
        _listener = new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down },
            { Keys.X, Controls.Jump } // TODO: find out why jumping currently is broken for the remote client (it probably has to do with a delta-time issue)
        });
    }

    public Guid Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; private set; }

    public void Follow(Func<Vector3> target)
    {
        // currently we don't have a need for this since the client camera doesn't move. It should not be called
        throw new NotImplementedException(); 
    }

    public void BeginDisplay()
    { 
        _renderer.Begin();
    }

    public void Display(Texture2D texture, Rectangle destination, float depth, Rectangle source, Color color, float rotation,
        Vector2 origin, SpriteEffects effect)
    {
        _renderer.Draw(texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Update(float deltaTime)
    {
        Controls = _listener.GetControls(this);
        
        _client.Send(Controls);
    }

    public void EndDisplay()
    {
        _renderer.End();
    }
}