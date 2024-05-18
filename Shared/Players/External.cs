using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Networking;
using Shared.Output;

namespace Shared.Players;

public class External : Player
{
    private readonly Server _server;

    public External(Guid userId, Camera perspective, Server server) : base(userId, perspective, server)
    {
        _server = server;
    }

    public override void BeginDisplay()
    {
        _server.PrepareRenderableBatch(this);
    }

    protected override void OnDisplay(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth)
    {
        _server.Enqueue(this, texture, destination, source, color, rotation, origin, effect, depth);
    }
    
    public override void EndDisplay()
    {
        _server.SendRenderableBatch(this);
    }
}