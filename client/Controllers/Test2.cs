using System;
using System.Collections.Generic;
using System.Diagnostics;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;
using SpatialPartition.Collision;

namespace client.Controllers;

public class Test2 : Game
{
    private readonly SpatialGrid<Ball> _spatialGrid;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Test2()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _spatialGrid = new SpatialGrid<Ball>();
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
        
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
        var ballTexture = Content.Load<Texture2D>("Test/ball");

        var random = new Random();
        const int maxBallSize = 5;

        for (var i = 0; i < 1000; i++)
        {
            var ballPosition =
                new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));

            var ball = new Ball(ballTexture, ballPosition, random.Next(1, 2),
                new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)), 
                random.Next(maxBallSize, maxBallSize));
            _spatialGrid.Add(ball);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _spatialGrid.Update(gameTime); // This will call Update on each Ball and handle spatial partitioning

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        foreach (var ball in _spatialGrid) ball.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

public class Ball : ICollidable
{
    private Color _color;
    private readonly Sprite _sprite;
    private Rectangle _destination;
    private Vector2 _velocity;

    public Ball(Texture2D texture, Vector2 position, float speed, Vector2 direction, int size)
    {
        _sprite = new Sprite(texture);
        Destination = new Rectangle((int)position.X, (int)position.Y, size, size);
        Velocity = direction * speed;
        _color = Color.White;
    }

    public Sprite Sprite => _sprite;
    public Color Color => _color;

    public Rectangle Destination
    {
        get => _destination;
        private init => _destination = value;
    }

    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    public void Update(GameTime gameTime)
    {
        // Example collision detection with screen bounds
        if (Destination.X < 0 || Destination.X > 2560 - Destination.Width) // Assuming screen width is 2560
            _velocity.X *= -1;
        if (Destination.Y < 0 || Destination.Y > 1440 - Destination.Height) // Assuming screen height is 1440
            _velocity.Y *= -1;

        _destination.X += (int)Velocity.X;// * gameTime.DeltaTime());
        _destination.Y += (int)Velocity.Y;// * gameTime.DeltaTime());
    }

    private static readonly HashSet<int> _collisions = new HashSet<int>();
    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation)
    {
        _color = Color.Red;
        Velocity = Vector2.Zero;
        
        if (!_collisions.Contains(Destination.Width))
        {
            Debug.WriteLine("Width = " + Destination.Width);
            _collisions.Add(Destination.Width);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Sprite.Texture, Destination, _color);
    }
}