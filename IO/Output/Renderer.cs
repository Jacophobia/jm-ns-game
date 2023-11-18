using System.Collections.Generic;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Output;

public class Renderer
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public Renderer(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
    }

    private void Draw(IRenderable renderable, Camera camera)
    {
        
        if (!renderable.Destination.Intersects(camera.View))
            return;
            
        var relativeDestination = new Rectangle(
            renderable.Destination.X - camera.View.X,
            renderable.Destination.Y - camera.View.Y,
            renderable.Destination.Width,
            renderable.Destination.Height
        );

        _spriteBatch.Draw(
            renderable.Texture,
            relativeDestination,
            renderable.Source,
            renderable.Color,
            renderable.Rotation,
            renderable.Origin,
            renderable.Effect,
            (renderable.Depth + 500) / 1000f
        );
    }

    public void Begin()
    {
        _graphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.BackToFront);
    }

    public void Render(IEnumerable<IRenderable> renderables, Camera camera)
    {
        foreach (var renderable in renderables) Draw(renderable, camera);
    }

    public void Render(IRenderable renderable, Camera camera)
    {
        Draw(renderable, camera);
    }

    public void FullRender(IRenderable renderable, Camera camera)
    {
        Begin();
        Draw(renderable, camera);
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