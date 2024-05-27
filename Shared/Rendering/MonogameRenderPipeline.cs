using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Rendering;

public class MonogameRenderPipeline
{
    private GraphicsDevice _contentManager;
    
    public SpriteBatch SpriteBatch { get; private set; }

    public ContentManager ContentManager { get; set; }
    public GraphicsDevice GraphicsDevice
    {
        get => _contentManager;
        set
        {
            SpriteBatch = new SpriteBatch(value);
            _contentManager = value;
        }
    }
    public GraphicsDeviceManager GraphicsDeviceManager { get; set; }
}