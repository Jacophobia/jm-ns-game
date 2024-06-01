using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Shared.Singletons;
using Shared.View;

namespace Shared.Rendering;

public class LocalRenderer : IRenderer
{
    private const int MaxDepth = 992;
    private const int MinDepth = -8;
    
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private bool _graphicsAreRendered;
    private bool _shouldClear;

    public LocalRenderer(GraphicsDevice graphicsDevice, ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        TextureManager.Initialize(contentManager);
        FontManager.Initialize(contentManager);
        _graphicsAreRendered = false;
        _shouldClear = true;
    }

    private static float AdjustDepth(float layerDepth)
    {
        return (MaxDepth - layerDepth) / 1000f;
    }

    private void Clear()
    {
        _graphicsDevice.Clear(Color.Black);
        _shouldClear = false;
    }

    public void Begin()
    {
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        _graphicsAreRendered = false;
    }
    
    public void Render(Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation, 
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

    public void Render(Camera camera, Texture2D texture, Rectangle destination, Rectangle source, Color color, float rotation,
        Vector2 origin, SpriteEffects effect, float depth)
    {
        if (!camera.CanSee(destination, depth))
        {
            return;
        }

        camera.Adjust(ref destination);

        Render(texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void End()
    {
        _spriteBatch.End();
        if (_graphicsAreRendered)
            _shouldClear = true;
    }
}