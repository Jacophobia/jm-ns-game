using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Networking;
using MonoGame.Output;

namespace MonoGame.Players;

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

    protected override void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null)
    {
        _server.Enqueue(this, renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    protected override void OnDisplay(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth)
    {
        _server.Enqueue(this, texture, destination, source, color, rotation, origin, effect, depth);
    }

    protected override void OnDisplay(IWritable writable, SpriteFont font = null, 
    string text = null, Vector2? position = null, Color? color = null, 
    float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, 
    float? depth = null)
    {
        _server.Enqueue(this, writable, font, text, position, color, rotation, origin, scale, effect, depth);
    }
    
    public override void EndDisplay()
    {
        _server.SendRenderableBatch(this);
    }
}