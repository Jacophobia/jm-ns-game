using SpatialPartition;
using SpatialPartition.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using IO.Sprites;

namespace client;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
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
        var ballTexture = Content.Load<Texture2D>("Test/ball");

        // Create and add some balls to the spatial grid
        for (var i = 0; i < 1000; i++)
        {
            var ball = new Ball(ballTexture, new Rectangle(_random.Next(2560), _random.Next(1440), 10, 10));
            _spatialGrid.Add(ball);
        }
    }

    protected override void Update(GameTime gameTime)
    {
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

    public Ball(Texture2D texture, Rectangle destination)
    {
        Sprite = new Sprite(texture);
        Destination = destination;
        var random = new Random();
        Velocity = new Vector2(random.Next(1, 6), random.Next(1, 4));
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