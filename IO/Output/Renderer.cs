using System.Collections.Generic;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Output;

public class Renderer
{
    public const int MaxDepth = 992;
    public const int MinDepth = -8;
    
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
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

    public void Begin()
    {
        _graphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.FrontToBack);
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

    public void FullRender(IRenderable renderable, Camera camera, Texture2D texture = null,
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        int? depth = null)
    {
        Begin();
        Draw(renderable, camera, texture, destination, source, color, rotation, origin, effect, depth);
        End();
    }

    public void FullRender(IEnumerable<IRenderable> renderables, Camera camera)
    {
        Begin();
        foreach (var renderable in renderables) Draw(renderable, camera);
        End();
    }

    public void End()
    {
        _spriteBatch.End();
    }
}