using System.Collections.Generic;
using System.Linq;
using jm_ns_game.Extensions;
using jm_ns_game.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace jm_ns_game;

public class MonoGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public MonoGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public void Begin()
    {
        Initialize();
        LoadContent();
    }

    public void Draw(GameTime gameTime, IEnumerable<IRenderable> drawables)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        foreach (var drawable in drawables)
        {
            _spriteBatch.Draw(drawable);
        }

        _spriteBatch.End();
        
        base.Draw(gameTime);
    }

    public Texture2D GetTexture(string name)
    {
        return Content.Load<Texture2D>(name);
    }

    public IEnumerable<Texture2D> GetTextures(IEnumerable<string> names)
    {
        return names.Select(name => Content.Load<Texture2D>(name));
    }

    public KeyboardState GetKeyboardState()
    {
        return Keyboard.GetState();
    }

    public MouseState GetMouseState()
    {
        return Mouse.GetState();
    }
}
