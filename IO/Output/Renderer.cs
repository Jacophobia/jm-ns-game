using System.Collections.Generic;
using IO.Extensions;
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

    public void Begin()
    {
        _graphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
    }

    public void Render(IEnumerable<IRenderable> renderables, Camera camera)
    {
        foreach (var renderable in renderables)
        {
            _spriteBatch.Draw(renderable, camera);
        }
    }

    public void Render(IRenderable renderable, Camera camera)
    {
        _spriteBatch.Draw(renderable, camera);
    }
    
    public void FullRender(IRenderable renderable, Camera camera)
    {
        Begin();
        _spriteBatch.Draw(renderable, camera);
        End();
    }

    public void FullRender(IEnumerable<IRenderable> renderables, Camera camera)
    {
        Begin();
        foreach (var renderable in renderables)
        {
            _spriteBatch.Draw(renderable, camera);
        }
        End();
    }

    public void End()
    {
        _spriteBatch.End();
    }
}