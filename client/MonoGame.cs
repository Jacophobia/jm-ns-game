using SpatialPartition;
using SpatialPartition.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using IO.Sprites;
using Microsoft.Xna.Framework.Input;

namespace client;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFactory _spriteFactory;

    private SpatialGrid<Ball> _spatialGrid;
    private Random _random;

    public Game1()
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
        _spatialGrid = new SpatialGrid<Ball>(12, 7);
        _random = new Random();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _spriteFactory = new SpriteFactory(Content);

        // Load textures for the balls
        var ballTexture = Content.Load<Texture2D>("Test/ball");// Get the primary graphics adapter
        
        using var adapter = GraphicsAdapter.DefaultAdapter;

        // Get the current display mode of the primary monitor
        var displayMode = adapter.CurrentDisplayMode;

        // Create and add some balls to the spatial grid
        for (var i = 0; i < 10000; i++)
        {
            var ball = new Ball(ballTexture, displayMode.Width, displayMode.Height, 2, 2);
            _spatialGrid.Add(ball);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        // Update the spatial grid, which will automatically update the balls 
        _spatialGrid.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Draw each ball
        foreach (var ball in _spatialGrid)
        {
            _spriteBatch.Draw(ball.Sprite.Texture, ball.Destination, ball.Color);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

public class Ball : ICollidable
{
    public Sprite Sprite { get; }
    public Rectangle Destination { get; private set; }

    public Color Color { get; private set; } = Color.Gray;


    private Vector2 _velocity;
    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    private readonly int _screenWidth;
    private readonly int _screenHeight;

    public Ball(Texture2D texture, int screenWidth, int screenHeight, int width, int height)
    {
        Sprite = new Sprite(texture);
        var random1 = new Random();
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        Destination = new Rectangle(random1.Next(_screenWidth), random1.Next(_screenHeight), width, height);
        Velocity = new Vector2(GetRandomNonZero(-6, 6), GetRandomNonZero(-4, 4));
    }

    private static int GetRandomNonZero(int min, int max)
    {
        var number = 0;
        
        while (number == 0)
        {
            number = new Random().Next(min, max);
        }

        return number;
    }

    public void Update()
    {
        // Move the ball (simplified movement for illustration purposes)
        var x = Destination.X + 1 * _velocity.X;
        var y = Destination.Y + 1 * _velocity.Y;

        if (x is > 2560 or < 0)
        {
            _velocity.X *= -1;
        }
        if (y is > 1440 or < 0)
        {
            _velocity.Y *= -1;
        }
        
        
        Destination = new Rectangle((int)x, (int)y, Destination.Width, Destination.Height);
    }

    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation)
    {
        // Handle collision logic, e.g., change color
        Color = Color.Red;

        _velocity = Vector2.Zero;
    }
}


// public class MonoGame : Game
// {
//     private readonly IEnumerable<IRenderable> _renderables;
//     private GraphicsDeviceManager _graphics;
//     private SpriteBatch _spriteBatch;
//
//     public MonoGame()
//     {
//         _renderables = Array.Empty<IRenderable>();
//         _graphics = new GraphicsDeviceManager(this);
//         Content.RootDirectory = "Content";
//         IsMouseVisible = true;
//     }
//
//     private Texture2D GetTexture(string name)
//     {
//         return Content.Load<Texture2D>(name);
//     }
//
//     private IEnumerable<Texture2D> GetTextures(IEnumerable<string> names)
//     {
//         return names.Select(name => Content.Load<Texture2D>(name));
//     }
//
//     protected override void Initialize()
//     {
//         // TODO: Add your initialization logic here
//
//         base.Initialize();
//     }
//
//     protected override void LoadContent()
//     {
//         _spriteBatch = new SpriteBatch(GraphicsDevice);
//     }
//
//     protected override void Update(GameTime gameTime)
//     {
//         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
//             Keyboard.GetState().IsKeyDown(Keys.Escape))
//         {
//             Exit();
//         }
//
//
//         base.Update(gameTime);
//     }
//
//     protected override void Draw(GameTime gameTime)
//     {
//         
//         base.Draw(gameTime);
//     }
// }