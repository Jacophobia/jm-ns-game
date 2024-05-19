using System.Collections.Generic;
using Combat.Arenas;
using Combat.Decorations;
using Combat.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shared.Controllers;
using Shared.Extensions;
using Shared.Input;
using Shared.Output;
using Shared.Players;
using Shared.Singletons;

namespace TestArena;

public class GameController : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private IPlayer _playerOne;
    private Arena _arena;

    private static readonly Dictionary<Keys, Controls> ControlMapping = new()
    {
        { Keys.A, Controls.Left },
        { Keys.E, Controls.Right },
        { Keys.OemComma, Controls.Up },
        { Keys.O, Controls.Down },
        { Keys.X, Controls.Jump }
    };

    public GameController(bool fullscreen = false)
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
    
        // Disable fixed time step
        IsFixedTimeStep = false;

        // Disable vertical retrace synchronization
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

        
        // Get the current display mode of the primary monitor
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var windowSize = adapter.CurrentDisplayMode;
        
        // Resize window
        _graphicsDeviceManager.PreferredBackBufferWidth = windowSize.Width;
        _graphicsDeviceManager.PreferredBackBufferHeight = windowSize.Height;
        
        // Set fullscreen mode
        _graphicsDeviceManager.IsFullScreen = fullscreen;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureManager.Initialize(Content);
        
        var spriteBatch = new SpriteBatch(GraphicsDevice);
        var renderer = new Renderer(GraphicsDevice, spriteBatch, Content);
        var controller = new PlayerController(ControlMapping);
        var fighter = new Fighter(controller, 100, 100);
        var camera = new Camera(fighter.GetCurrentPosition, 0.5f, Vector3.Backward * 10f);
        _playerOne = new Host(camera, renderer);

        _arena = new Arena(ArenaType.Infinte, new List<Decoration>());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _arena.Update(gameTime);
        _playerOne.Update(gameTime.DeltaTime());

        base.Update(gameTime);
    }

    protected override bool BeginDraw()
    {
        _playerOne.BeginDisplay();
        
        return base.BeginDraw();
    }

    protected override void Draw(GameTime gameTime)
    {
        _arena.Render(_playerOne);

        base.Draw(gameTime);
    }

    protected override void EndDraw()
    {
        _playerOne.EndDisplay();
        
        base.EndDraw();
    }
}
