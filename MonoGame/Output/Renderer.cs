using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Singletons;

namespace MonoGame.Output;

public class Renderer
{
    internal const int MaxDepth = 992;
    internal const int MinDepth = -8;
    
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    internal Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        TextureManager.Initialize(contentManager);
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

        _spriteBatch.Draw(
            texture ?? renderable.Texture,
            relativeDestination,
            source ?? renderable.Source,
            color ?? renderable.Color,
            rotation ?? renderable.Rotation,
            origin ?? renderable.Origin,
            effect == SpriteEffects.None ? renderable.Effect : effect,
            (MaxDepth - (depth ?? renderable.Depth)) / 1000f
        );
    }

    public void Render(IEnumerable<IRenderable> renderables, Camera camera)
    {
        foreach (var renderable in renderables) Draw(renderable, camera);
    }

    public void Render(IRenderable renderable, Camera camera, Texture2D texture = null,
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        Draw(renderable, camera, texture, destination, source, color, rotation, origin, effect, depth);
    }
}