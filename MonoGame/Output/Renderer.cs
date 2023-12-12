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
    private bool _graphicsAreRendered;
    private bool _shouldClear;

    internal Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager)
    {
         _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        TextureManager.Initialize(contentManager);
        _graphicsAreRendered = false;
        _shouldClear = true;
    }

    public void Draw(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null)
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
    
    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, destinationRectangle, color);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Vector2 position, Color color)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, position, color);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, position, sourceRectangle, color);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, 
        float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, 
            (MaxDepth - layerDepth) / 1000f);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, 
        float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, 
            (MaxDepth - layerDepth) / 1000f);

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, 
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, 
            (MaxDepth - layerDepth) / 1000f);

        _graphicsAreRendered = true;
    }
    
    public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, 
        Vector2 origin, SpriteEffects effect, float depth)
    {
        if (_shouldClear)
            Clear();
        
        _spriteBatch.Draw(
            texture,
            destination,
            source,
            color,
            rotation,
            origin,
            effect,
            (MaxDepth - depth) / 1000f
        );

        _graphicsAreRendered = true;
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
        _spriteBatch.End();
        if (_graphicsAreRendered)
            _shouldClear = true;
    }
}