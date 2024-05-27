using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Networking;
using Shared.View;

namespace Shared.Rendering;

public class RemoteRenderer : IRenderer
{
    public readonly Guid PlayerId;
    private readonly Server _server;

    public RemoteRenderer(Server server, Guid playerId)
    {
        _server = server;
        PlayerId = playerId;
    }
    
    public void Begin()
    {
        _server.PrepareRenderableBatch(PlayerId);
    }

    public void Render(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth)
    {
        _server.Enqueue(PlayerId, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Render(Camera camera, Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation,
        Vector2 origin, SpriteEffects effect, float depth)
    {
        if (!camera.CanSee(destination, depth))
        {
            return;
        }
        
        camera.Adjust(ref destination);
        
        _server.Enqueue(PlayerId, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void End()
    {
        _server.SendRenderableBatch(PlayerId);
    }
}