using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.DataStructures;
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
    private readonly PriorityQueue<Renderable> _networkRenderables;
    private readonly ObjectPool<Renderable> _renderablePool;
    private bool _graphicsAreRendered;
    private bool _shouldClear;

    internal Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager, NetworkClient networkClient)
    {
         _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        TextureManager.Initialize(contentManager);
        _networkClient = networkClient;
        _networkRenderables = new PriorityQueue<Renderable>();
        _renderablePool = new ObjectPool<Renderable>();
        _graphicsAreRendered = false;
        _shouldClear = true;
    }
    
    private void Draw(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        if (_shouldClear)
            Clear();
        
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

        _graphicsAreRendered = true;
    }
    
    private void Send(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        var sendableRenderable = _renderablePool.Get();
        
        sendableRenderable.Texture = texture ?? renderable.Texture;
        sendableRenderable.Destination = destination ?? renderable.Destination;
        sendableRenderable.Source = source ?? renderable.Source;
        sendableRenderable.Color = color ?? renderable.Color;
        sendableRenderable.Rotation = rotation ?? renderable.Rotation;
        sendableRenderable.Origin = origin ?? renderable.Origin;
        sendableRenderable.Effect = effect == SpriteEffects.None ? renderable.Effect : effect;
        sendableRenderable.Depth = depth ?? renderable.Depth;
        
        _networkRenderables.Put(sendableRenderable, _networkClient.TotalMilliseconds);
    }

    private void SendAllRenderables()
    {
        if (_networkRenderables.IsEmpty) return;
        
        var renderables = _networkRenderables.GetAll().ToArray();
        _networkClient.SendRenderableData(renderables);
        foreach (var renderable in renderables)
            _renderablePool.Return(renderable);
    }

    private void Draw(IRenderable renderable, Camera camera, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(camera.View))
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

    private void Clear()
    {
        _graphicsDevice.Clear(Color.Black);
        _shouldClear = false;
    }

    internal void Begin()
    {
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        _graphicsAreRendered = false;
    }

    internal void End()
    {
        SendAllRenderables();
        _spriteBatch.End();
        if (_graphicsAreRendered)
            _shouldClear = true;
    }
}