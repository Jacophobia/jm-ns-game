using System;
using System.Collections.Generic;
using System.Diagnostics;
using IO.Extensions;
using IO.Input;
using IO.Output;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpatialPartition;
using SpatialPartition.Collision;
using SpatialPartition.Interfaces;

namespace client.Controllers;

public class Test2 : Game
{
    private readonly ISpatialPartition<Ball> _spatialGrid;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Listener _listener;
    private Camera _camera;
    private Texture2D _background;
    private Rectangle _backgroundSize;

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
        _listener = new Listener(new Dictionary<Buttons, Controls>());
        _background = new Texture2D(GraphicsDevice, 1, 1);
        _background.SetData(new[] { Color.Black }); // Set the pixel to black
        var ballTexture = Content.Load<Texture2D>("Test/ball");

        var random = new Random();
        const int minBallSize = 1;
        const int maxBallSize = 50;

        for (var i = 0; i < 100; i++)
        {
            var ballPosition =
                new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
            Ball ball;
            if (i == 0)
            {
                ball = new Ball(ballTexture, ballPosition, random.Next(1, 15),
                    new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)), 
                    (int)(maxBallSize * 1.5));
                _camera = new Camera(ball, 1, Vector3.Zero);
            }
            else
            {
                ball = new Ball(ballTexture, ballPosition, random.Next(1, 15),
                    new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)), 
                    random.Next(minBallSize, maxBallSize));
            }
            _spatialGrid.Add(ball);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _spatialGrid.Update(gameTime, _listener.GetInputState()); // This will call Update on each Ball and handle spatial partitioning
        _camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, _backgroundSize, Color.Black, _camera);
        _spatialGrid.Draw(_spriteBatch, _camera, gameTime);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

public class Ball : ICollidable
{
    public Sprite Sprite { get; }
    public Rectangle Source { get; }

    public Color Color
    {
        get => _color;
        private set => _color = value;
    }

    public float Rotation { get; }
    public Vector2 Origin { get; }
    public SpriteEffects Effect { get; }
    public float Depth { get; }
    public Texture2D Texture => Sprite.Texture;
    public Rectangle Destination
    {
        get => _destination;
        set => _destination = value;
    }
    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }
    
    private Rectangle _destination;
    private Vector2 _velocity;
    private Color _color;

    public Ball(Texture2D texture, Vector2 position, float speed, Vector2 direction, int size)
    {
        Sprite = new Sprite(texture);
        Destination = new Rectangle((int)position.X, (int)position.Y, size, size);
        Velocity = direction * speed;
        Color = Color.White.ToStepped();
        Source = texture.Bounds;
        Rotation = 0f;
        Origin = Vector2.Zero;
        Effect = SpriteEffects.None;
        Depth = 0f;
        
    }

    public void Update(GameTime gameTime, Controls controls)
    {
        // Example collision detection with screen bounds
        if (Destination.X < 0) // Assuming screen width is 2560
            _velocity.X = MathF.Abs(_velocity.X);
        if (Destination.X > 2560 - Destination.Width)
            _velocity.X = MathF.Abs(_velocity.X) * -1;
        if (Destination.Y < 0)
            _velocity.Y = MathF.Abs(_velocity.Y);
        if (Destination.Y > 1440 - Destination.Height) // Assuming screen height is 1440
            _velocity.Y = MathF.Abs(_velocity.Y) * -1;

        _destination.X += (int)Velocity.X;// * gameTime.DeltaTime());
        _destination.Y += (int)Velocity.Y;// * gameTime.DeltaTime());
    }
    
    private void RewindOneFrame(ICollidable collidable)
    { 
        var myDestination = Destination;
        var otherDestination = collidable.Destination;

        myDestination.X -= (int)MathF.Ceiling(Velocity.X);
        myDestination.Y -= (int)MathF.Ceiling(Velocity.Y);

        otherDestination.X -= (int)MathF.Ceiling(collidable.Velocity.X);
        otherDestination.Y -= (int)MathF.Ceiling(collidable.Velocity.Y);

        Destination = myDestination;
        collidable.Destination = otherDestination;
    }

    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap)
    {
        RewindOneFrame(collidable);

        var color = Color;
        var velocity = Velocity;
        
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        
        color.R++;
        color.G++;
        color.B++;
        // (_color.R, _color.G, _color.B) = (Color.G, Color.B, Color.R);

        // Calculate the normal (n) and tangential (t) direction vectors
        float nx = collidable.Destination.Center.X - Destination.Center.X;
        float ny = collidable.Destination.Center.Y - Destination.Center.Y;
        var distance = MathF.Sqrt(nx * nx + ny * ny);
        nx /= distance; // Normalize
        ny /= distance; // Normalize

        // Decompose velocities into normal and tangential components
        var v1n = Velocity.X * nx + Velocity.Y * ny; // Dot product
        var v1t = -Velocity.X * ny + Velocity.Y * nx; // Perpendicular dot product
        var v2n = collidable.Velocity.X * nx + collidable.Velocity.Y * ny;
        var v2t = -collidable.Velocity.X * ny + collidable.Velocity.Y * nx;

        // Exchange normal components in elastic collision
        (v1n, v2n) = (v2n, v1n);

        // Recompose velocities
        velocity.X = v1n * nx - v1t * ny;
        velocity.Y = v1n * ny + v1t * nx;
        collidable.Velocity = new Vector2(v2n * nx - v2t * ny, v2n * ny + v2t * nx);

        Color = color;
        Velocity = velocity;
    }
}