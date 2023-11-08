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

    public void Render(IEnumerable<IRenderable> renderables)
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (var renderable in renderables) _spriteBatch.Draw(renderable);

        _spriteBatch.End();
    }
}