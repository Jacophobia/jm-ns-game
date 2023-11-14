using System;
using System.Collections.Generic;
using System.Diagnostics;
using IO.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;

namespace client.Controllers;

public class Test1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpatialGrid<Ball> _spatialGrid;
    private SpriteBatch _spriteBatch;
    private Listener _listener;

    public Test1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;

        using var adapter = GraphicsAdapter.DefaultAdapter;

        // Get the current display mode of the primary monitor
        var displayMode = adapter.CurrentDisplayMode;

        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;

        // Set fullscreen mode
        _graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        // Initialize the spatial grid with 10x10 partitions
        _spatialGrid = new SpatialGrid<Ball>();

        base.Initialize();
    }

    private static int GetNonZeroRandom(int min, int max)
    {
        Debug.Assert(min != 0 && max != 0);
        var random = new Random();
        var randomNum = 0;
        while (randomNum == 0) randomNum = random.Next(min, max);
        return randomNum;
    }

    private static int GetOddRandom(int min, int max)
    {
        Debug.Assert(min != max || min % 2 == 1);
        var random = new Random();
        var randomNum = 0;
        while (randomNum % 2 != 1) randomNum = random.Next(min, max);
        return randomNum;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _listener = new Listener(new Dictionary<Buttons, Controls>());

        // Load textures for the balls
        var ballTexture = Content.Load<Texture2D>("Test/ball"); // Get the primary graphics adapter

        using var adapter = GraphicsAdapter.DefaultAdapter;

        // Get the current display mode of the primary monitor
        var displayMode = adapter.CurrentDisplayMode;

        // Create and add some balls to the spatial grid
        var random = new Random();
        for (var i = 0; i < 1000; i++) // TODO: Odd numbers below 9 are not colliding. Fix it
        {
            var oddNum = i % 22 + 1;
            var ball = new Ball(ballTexture,
                new Vector2(random.Next(displayMode.Width), random.Next(displayMode.Height)), random.Next(1, 2),
                new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)), oddNum);
            _spatialGrid.Add(ball);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // Update the spatial grid, which will automatically update the balls 
        _spatialGrid.Update(gameTime, _listener.GetInputState());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Draw each ball
        foreach (var ball in _spatialGrid) _spriteBatch.Draw(ball.Sprite.Texture, ball.Destination, ball.Color);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}