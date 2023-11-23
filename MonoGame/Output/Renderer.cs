using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Networking;
using MonoGame.Singletons;

namespace MonoGame.Output;

public class Renderer
{
    internal const int MaxDepth = 992;
    internal const int MinDepth = -8;
    
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly NetworkClient _networkClient;

    internal Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager, NetworkClient networkClient)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        TextureManager.Initialize(contentManager);
        _networkClient = networkClient;
    }
    
    private void Draw(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        _spriteBatch.Draw(
            texture ?? renderable.Texture,
            destination ?? renderable.Destination,
            source ?? renderable.Source,
            color ?? renderable.Color,
            rotation ?? renderable.Rotation,
            origin ?? renderable.Origin,
            effect == SpriteEffects.None ? renderable.Effect : effect,
            (MaxDepth - (depth ?? renderable.Depth)) / 1000f
        );
    }
    
    private void Send(IRenderable renderable, GraphicsResource texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        _networkClient.SendRenderableData(new Renderable(
            texture?.Name ?? renderable.Texture.Name,
            destination ?? renderable.Destination,
            source ?? renderable.Source,
            color ?? renderable.Color,
            rotation ?? renderable.Rotation,
            origin ?? renderable.Origin,
            effect == SpriteEffects.None ? renderable.Effect : effect,
            depth ?? renderable.Depth
        ));
    }

    private void Draw(IRenderable renderable, Camera camera, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        if (!renderable.Destination.Intersects(camera.View))
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= camera.View.X;
        relativeDestination.Y -= camera.View.Y;

        if (camera.ClientIndex == 0)
            Draw(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
        else
            Send(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

    public void Render(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        Draw(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }
    
    public void Render(IRenderable renderable, IEnumerable<Camera> cameras, Texture2D texture = null,
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        foreach (var camera in cameras)
            Draw(renderable, camera, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Render(IRenderable renderable, Camera camera, Texture2D texture = null,
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        Draw(renderable, camera, texture, destination, source, color, rotation, origin, effect, depth);
    }
}