using System;
using System.Collections.Generic;
using System.Diagnostics;
using client.Decorators;
using client.Entities;
using IO.Extensions;
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
    private readonly ISpatialPartition<Entity> _spatialGrid;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Renderer _renderer;
    private Listener _listener;
    private Camera _camera;
    private Texture2D _background;
    private Rectangle _backgroundSize;

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
        _listener = new Listener(new Dictionary<Buttons, Controls>()); // TODO: Put in the actual control mappings
        _renderer = new Renderer(GraphicsDevice, _spriteBatch);
        _background = new Texture2D(GraphicsDevice, 1, 1);
        _background.SetData(new[] { Color.Black }); // Set the pixel to black
        var ballTexture = Content.Load<Texture2D>("Test/ball");

        var random = new Random();
        const int minBallSize = 1;
        const int maxBallSize = 5;
        const int numBalls = 26;

        for (var i = 0; i < numBalls; i++)
        {
            var ballPosition =
                new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
            var size = random.Next(minBallSize, maxBallSize);

            var entity = new EntityBuilder()
                .SetColor(Color.White)
                .SetDepth(0f)
                .SetEffect(SpriteEffects.None)
                .SetTexture(ballTexture)
                .SetSource(ballTexture.Bounds)
                .SetWidth(size * (numBalls - i))
                .SetHeight(size * (numBalls - i))
                .SetPosition(ballPosition)
                .SetOrigin(new Point(size / 2, size / 2))
                .SetRotation(0f)
                .SetVelocity(new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) * (1f / 0.016f))
                .SetRestitutionCoefficient(0.8f)
                .AddDecorator<Bound>(new Rectangle(0, 0, 2560, 1440))
                .AddDecorator<Elastic>()
                .AddDecorator<Inertia>()
                .AddDecorator<Drag>(0.001f)
                .Build();

            if (i == 0)
            {
                var newDestination = entity.Destination;
                newDestination.Size = new Point(maxBallSize * 2, maxBallSize * 2);
                _camera = new Camera(entity, 1, Vector3.Up * 100);
            }
            
            _spatialGrid.Add(entity);
        }
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
        _spriteBatch.Draw(_background, _backgroundSize, Color.Black, _camera);
        _spatialGrid.Draw(_renderer, _camera, gameTime);
        _renderer.End();

        base.Draw(gameTime);
    }
}

