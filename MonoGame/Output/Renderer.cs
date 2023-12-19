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
        FontManager.Initialize(contentManager);
        _graphicsAreRendered = false;
        _shouldClear = true;
    }

    private static float AdjustDepth(float layerDepth)
    {
        return (MaxDepth - layerDepth) / 1000f;
    }

    /// <summary>
    /// Draws a string with additional parameters like rotation, origin, scale, effects, and layer depth.
    /// </summary>
    /// <param name="writable">The writable object you would like to display</param>
    /// <param name="font">The font used to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="position">The top-left position to draw the text at.</param>
    /// <param name="color">The color to apply to the text.</param>
    /// <param name="rotation">The angle (in radians) to rotate the text around the origin.</param>
    /// <param name="origin">The point to rotate around and to scale from.</param>
    /// <param name="scale">The scale factor.</param>
    /// <param name="effects">Effects to apply (like flipping).</param>
    /// <param name="layerDepth">The depth of the layer where the text is drawn (between 0 [front] and 1 [back]).</param>
    public void Write(IWritable writable, SpriteFont font = null, string text = null, Vector2? position = null, Color? color = null, float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects? effects = null, float? layerDepth = null)
    {
        _spriteBatch.DrawString(
            font ?? writable.Font, 
            text ?? writable.Text, 
            position ?? writable.Position, 
            color ?? writable.TextColor, 
            rotation ?? writable.Rotation, 
            origin ?? writable.Origin, 
            scale ?? writable.Scale, 
            effects ?? writable.Effects, 
            AdjustDepth(layerDepth ?? writable.LayerDepth)
        );
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
            AdjustDepth(depth ?? renderable.Depth)
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
            AdjustDepth(layerDepth));

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, 
        float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, 
            AdjustDepth(layerDepth));

        _graphicsAreRendered = true;
    }

    public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, 
        float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        if (_shouldClear)
            Clear();

        _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, 
            AdjustDepth(layerDepth));

        _graphicsAreRendered = true;
    }
    
    public void Draw(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, 
        Vector2 origin, SpriteEffects effect, float layerDepth)
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
            AdjustDepth(layerDepth)
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