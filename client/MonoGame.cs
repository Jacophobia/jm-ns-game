using System;
using client.Extensions;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;
using SpatialPartition.Collision;

namespace client;

public class Game1 : Game
{
    private readonly SpatialGrid<Ball> _spatialGrid;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _spatialGrid = new SpatialGrid<Ball>();
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
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

        for (var i = 0; i < 10; i++)
        {
            var ballPosition =
                new Vector2(random.Next(2560 - ballTexture.Width), random.Next(1440 - ballTexture.Height));

            var ball = new Ball(ballTexture, ballPosition, random.Next(6),
                new Vector2(GetNonZeroRandom(-10, 10), GetNonZeroRandom(-10, 10)), 
                random.Next(1, 10));
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
    private readonly float _speed;
    private Color _color;
    private Vector2 _direction;

    public Ball(Texture2D texture, Vector2 position, float speed, Vector2 direction, int size)
    {
        Sprite = new Sprite(texture);
        Destination = new Rectangle((int)position.X, (int)position.Y, size, size);
        _speed = speed;
        _direction = direction;
        _color = Color.White;
    }

    public Sprite Sprite { get; }
    public Rectangle Destination { get; private set; }
    public Vector2 Velocity { get; set; }

    public void Update(GameTime gameTime)
    {
        // Example collision detection with screen bounds
        if (Destination.X < 0 || Destination.X > 2560 - Destination.Width) // Assuming screen width is 2560
            _direction.X *= -1;
        if (Destination.Y < 0 || Destination.Y > 1440 - Destination.Height) // Assuming screen height is 1440
            _direction.Y *= -1;

        var newDestination = Destination;
        newDestination.X += (int)(_direction.X * _speed);// * gameTime.DeltaTime());
        newDestination.Y += (int)(_direction.Y * _speed);// * gameTime.DeltaTime());
        Destination = newDestination;
    }

    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation)
    {
        _color = Color.Red;
        Velocity = Vector2.Zero;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Sprite.Texture, Destination, _color);
    }
}