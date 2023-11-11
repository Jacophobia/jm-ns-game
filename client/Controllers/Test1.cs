using System;
using System.Diagnostics;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;
using SpatialPartition.Collision;

namespace client.Controllers;

public class Test1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpatialGrid<Ball> _spatialGrid;
    private SpriteBatch _spriteBatch;

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
        _spatialGrid.Update(gameTime);

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

public class Ball1 : ICollidable
{
    private readonly int _screenHeight;

    private readonly int _screenWidth;
    private Rectangle _destination;

    private Vector2 _velocity;

    public Ball1(Texture2D texture, int screenWidth, int screenHeight, int width, int height)
    {
        Sprite = new Sprite(texture);
        var random1 = new Random();
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _destination = new Rectangle(random1.Next(_screenWidth), random1.Next(_screenHeight), width, height);
        _velocity = new Vector2(GetRandomNonZero(-6, 6), GetRandomNonZero(-4, 4));
    }

    public Color Color { get; private set; } = Color.Gray;
    public Sprite Sprite { get; }

    public Rectangle Destination => _destination;

    public void Update(GameTime gameTime)
    {
        // Move the ball (simplified movement for illustration purposes)
        var x = Destination.X * _velocity.X;
        var y = Destination.Y * _velocity.Y;

        if (x is > 2560 or < 0) _velocity.X *= -1;
        if (y is > 1440 or < 0) _velocity.Y *= -1;

        _destination.X = (int)x;
        _destination.Y = (int)y;
    }

    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation)
    {
        // Handle collision logic, e.g., change color
        Color = Color.Red;

        _velocity = Vector2.Zero;
    }

    private static int GetRandomNonZero(int min, int max)
    {
        var number = 0;

        while (number == 0) number = new Random().Next(min, max);

        return number;
    }
}