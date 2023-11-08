using System;
using System.Collections.Generic;
using System.Linq;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace client;

public class MonoGame : Game
{
    private readonly IEnumerable<IRenderable> _renderables;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public MonoGame()
    {
        _renderables = Array.Empty<IRenderable>();
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    private Texture2D GetTexture(string name)
    {
        return Content.Load<Texture2D>(name);
    }

    private IEnumerable<Texture2D> GetTextures(IEnumerable<string> names)
    {
        return names.Select(name => Content.Load<Texture2D>(name));
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        
        base.Draw(gameTime);
    }
}