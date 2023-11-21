using System;
using System.Collections.Generic;
using client.Decorators;
using client.Entities;
using client.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;
using SpatialPartition.Interfaces;

namespace client.Controllers;

public class Test2 : Game
{
    private readonly Rectangle _backgroundSize;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly GraphicsDeviceManager _graphics;
    private readonly ISpatialPartition<Entity> _spatialGrid;
    private List<Entity> _background;
    private Camera _camera;
    private Listener _listener;
    private Renderer _renderer;
    private SpriteBatch _spriteBatch;

    public Test2()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _spatialGrid = new SpatialGrid<Entity>();
        Window.AllowUserResizing = true;
        IsMouseVisible = true;

        using var adapter = GraphicsAdapter.DefaultAdapter;

        // Get the current display mode of the primary monitor
        var displayMode = adapter.CurrentDisplayMode;

        _backgroundSize = new Rectangle(0, 0, displayMode.Width, displayMode.Height);

        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;

        // Set fullscreen mode
        _graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    private static int GetNonZeroRandom(int min, int max)
    {
        var random = new Random();
        var randomNum = 0;
        while (randomNum == 0) randomNum = random.Next(min, max);
        return randomNum;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _listener = new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down }
        });
        _renderer = new Renderer(GraphicsDevice, _spriteBatch);
        SetBackground();
        var ballTexture = Content.Load<Texture2D>("Test/ball");

        var random = new Random();
        const int minBallSize = 1;
        const int maxBallSize = 5;
        const int numBalls = 50;

        for (var i = -1; i < numBalls; i++)
        {
            var color = new Color(random.Next(200), random.Next(255), random.Next(255));
            for (var j = 0; j < 10; j++)
            {
                var ballPosition =
                    new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
                var size = random.Next(minBallSize, maxBallSize);

                var entityBuilder = new EntityBuilder(
                        ballTexture,
                        ballPosition,
                        new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) *
                        (1f / 0.016f),
                        50,
                        50)
                    .SetDepth(i * 5)
                    .SetColor(color)
                    .AddDecorator<Inertia>()
                    .AddDecorator<CircularCollision>()
                    .AddDecorator<Bound>(new Rectangle(0, 0, 2560, 1440))
                    .AddDecorator<PerspectiveRender>(true, -10);

                if (i == 0)
                {
                    entityBuilder.SetColor(Color.Red);
                    entityBuilder.SetDepth(0);
                    var entity = entityBuilder.Build();
                    _camera = new Camera(entity, 1, Vector3.Up * 100);
                    _spatialGrid.Add(entity);
                    continue;
                }

                _spatialGrid.Add(entityBuilder.Build());
            }
        }
    }

    private void SetBackground()
    {
        var backgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
        backgroundTexture.SetData(new[] { Color.White }); // Set the pixel to black
        backgroundTexture.Name = "Background";
        _background = new List<Entity>();
        foreach (var side in _backgroundSize.GetOutline(50).GetSides())
            for (var i = -1; i < 50; i++)
                _background.Add(new EntityBuilder(
                        backgroundTexture,
                        new Vector2(side.X, side.Y),
                        Vector2.Zero,
                        side.Width,
                        side.Height)
                    .SetDepth(5 * i)
                    .SetStatic(true)
                    .SetColor(Color.White)
                    .AddDecorator<PerspectiveRender>(true, -10)
                    .Build());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var controls = _listener.GetInputState();
        _spatialGrid.Update(gameTime, controls); // This will call Update on each Ball and handle spatial partitioning
        _camera.Update(gameTime, controls);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.Begin();
        foreach (var side in _background) side.Draw(_renderer, _camera);
        _spatialGrid.Draw(_renderer, _camera, gameTime);
        _renderer.End();

        base.Draw(gameTime);
    }
}