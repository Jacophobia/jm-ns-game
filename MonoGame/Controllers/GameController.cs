using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extensions;
using MonoGame.Output;

namespace MonoGame.Controllers;

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
public abstract class GameController : Game
{
    protected Renderer Renderer; // TODO: Make this class more generic and have two new abstract child classes of it which implement networking features for the host and thin clients
    protected SpriteBatch SpriteBatch;

    protected static Rectangle WindowSize
    {
        get
        {
            using var adapter = GraphicsAdapter.DefaultAdapter;

            // Get the current display mode of the primary monitor
            var displayMode = adapter.CurrentDisplayMode;

            return new Rectangle(0, 0, displayMode.Width, displayMode.Height);
        }
    }

    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    
    protected GameController(bool fullscreen)
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        IsMouseVisible = true;
    
        // Disable fixed time step
        IsFixedTimeStep = false;

        // Disable vertical retrace synchronization
        _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

        var windowSize = WindowSize;
        
        _graphicsDeviceManager.PreferredBackBufferWidth = windowSize.Width;
        _graphicsDeviceManager.PreferredBackBufferHeight = windowSize.Height;
        // Set fullscreen mode
        _graphicsDeviceManager.IsFullScreen = fullscreen;
    }

    /// <summary>
    /// Initializes game component. Called once before the game loop
    /// starts and after the graphics device is initialized.
    /// </summary>
    protected virtual void OnInitialize() {}

    protected internal virtual void BeforeOnInitialize() {}

    protected internal virtual void AfterOnInitialize()
    {
        if (Renderer == null)
        {
            throw new MissingMemberException($"Renderer was not set in {nameof(BeforeOnInitialize)}");
        }
    }
    protected sealed override void Initialize()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        
        BeforeOnInitialize();
        OnInitialize();
        AfterOnInitialize();
        
        base.Initialize();
    }

    /// <summary>
    /// Called when the game should load its content.
    /// </summary>
    protected virtual void OnLoadContent() {}
    protected internal virtual void BeforeOnLoadContent() {}
    protected internal virtual void AfterOnLoadContent() {}
    protected sealed override void LoadContent()
    {
        BeforeOnLoadContent();
        OnLoadContent();
        AfterOnLoadContent();

        base.LoadContent();
    }
    
    /// <summary>
    /// Called once before the game loop begins. Useful for any one-time
    /// setup.
    /// </summary>
    protected virtual void OnBeginRun() {}
    protected internal virtual void BeforeOnBeginRun() {}
    protected internal virtual void AfterOnBeginRun() {}
    protected sealed override void BeginRun()
    {
        BeforeOnBeginRun();
        OnBeginRun();
        AfterOnBeginRun();
        base.BeginRun();
        
        #if DEBUG
        _totalRuntimeStopwatch.Start();
        #endif
    }

    /// <summary>
    /// Called each frame to update the game logic.
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OnUpdate(float deltaTime) {}
    protected internal virtual void BeforeOnUpdate(float deltaTime) {}
    protected internal virtual void AfterOnUpdate(float deltaTime) {}
    protected sealed override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        var deltaTime = gameTime.DeltaTime();
        
        if (deltaTime == 0f)
        {
            base.Update(gameTime);
            return;
        }

        BeforeOnUpdate(deltaTime);
        OnUpdate(deltaTime);
        AfterOnUpdate(deltaTime);
        base.Update(gameTime);

        #if DEBUG
        _updateCallCount++;
        #endif
    }

    /// <summary>
    /// Called before drawing the frame. Allows any necessary pre-draw
    /// operations.
    /// </summary>
    protected virtual void OnBeginDraw() {}
    protected internal virtual void BeforeOnBeginDraw() {}
    protected internal virtual void AfterOnBeginDraw() {}
    protected sealed override bool BeginDraw()
    {
        BeforeOnBeginDraw();
        OnBeginDraw();
        AfterOnBeginDraw();
        return base.BeginDraw();
    }

    /// <summary>
    /// Called when the game should draw itself.
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OnDraw(float deltaTime) {}
    protected internal virtual void BeforeOnDraw(float deltaTime) {}
    protected internal virtual void AfterOnDraw(float deltaTime) {}
    protected sealed override void Draw(GameTime gameTime)
    {
        var deltaTime = gameTime.DeltaTime();

        if (deltaTime == 0f)
        {
            base.Draw(gameTime);
            return;
        }
        
        BeforeOnDraw(deltaTime);
        OnDraw(deltaTime);
        AfterOnDraw(deltaTime);
        base.Draw(gameTime);
    }

    /// <summary>
    /// Called after drawing the frame. Allows any necessary post-draw
    /// operations.
    /// </summary>
    protected virtual void OnEndDraw() {}
    protected internal virtual void BeforeOnEndDraw() {}
    protected internal virtual void AfterOnEndDraw() {}
    protected sealed override void EndDraw()
    {
        BeforeOnEndDraw();
        OnEndDraw();
        AfterOnEndDraw();
        base.EndDraw();
    }

    /// <summary>
    /// Called once after the game loop ends. Useful for any cleanup.
    /// </summary>
    protected virtual void OnEndRun() {}
    protected internal virtual void BeforeOnEndRun() {}
    protected internal virtual void AfterOnEndRun() {}
    protected sealed override void EndRun()
    {
        BeforeOnEndRun();
        OnEndRun();
        AfterOnEndRun();
        base.EndRun();
        
        #if DEBUG
        _totalRuntimeStopwatch.Stop();
        #endif
    }

    /// <summary>
    /// Called when the game should unload its content.
    /// </summary>
    protected virtual void OnUnloadContent() {}
    protected internal virtual void BeforeOnUnloadContent() {}
    protected internal virtual void AfterOnUnloadContent() {}
    protected sealed override void UnloadContent()
    {
        BeforeOnUnloadContent();
        OnUnloadContent();
        AfterOnUnloadContent();
        base.UnloadContent();
    }

    /// <summary>
    /// Called when the game is exiting. Triggers the OnExit method for
    /// any necessary cleanup.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Arguments for the event.</param>
    protected virtual void OnExit(object sender, EventArgs args) {}
    protected internal virtual void BeforeOnExit(object sender, EventArgs args) {}
    protected internal virtual void AfterOnExit(object sender, EventArgs args) {}
    protected sealed override void OnExiting(object sender, EventArgs args)
    {
        BeforeOnExit(sender, args);
        OnExit(sender, args);
        AfterOnExit(sender, args);
        base.OnExiting(sender, args);
    }
    

    /// <summary>
    /// Disposes of the game's resources. Called when the game is being
    /// destroyed.
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether the method call comes from a Dispose method
    /// (its value is true) or from a finalizer (its value is false).
    /// </param>
    protected virtual void OnDispose(bool disposing) {}
    protected internal virtual void BeforeOnDispose(bool disposing) {}
    protected internal virtual void AfterOnDispose(bool disposing) {}
    protected sealed override void Dispose(bool disposing)
    {
        #if DEBUG
        _totalRuntimeStopwatch.Stop();
        var totalSeconds = _totalRuntimeStopwatch.Elapsed.TotalSeconds;
        if (!(totalSeconds > 0)) return;
        var updatesPerSecond = _updateCallCount / totalSeconds;
        Debug.WriteLine($"Average Updates per Second: {updatesPerSecond}");
        Console.WriteLine($"Average Updates per Second: {updatesPerSecond}");
        #endif
        
        BeforeOnDispose(disposing);
        OnDispose(disposing);
        AfterOnDispose(disposing);
        base.Dispose(disposing);
    }
    
    /// <summary>
    /// Called when the game window gains focus. This method is used to
    /// trigger any actions that should occur when the game becomes the
    /// active window, such as resuming gameplay or animations.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, typically the game window.
    /// </param>
    /// <param name="args">
    /// Arguments for the event, containing any additional info about
    /// the activation.
    /// </param>
    protected virtual void OnWindowFocused(object sender, EventArgs args) {}
    protected internal virtual void BeforeOnWindowFocused(object sender, EventArgs args) {}
    protected internal virtual void AfterOnWindowFocused(object sender, EventArgs args) {}
    protected sealed override void OnActivated(object sender, EventArgs args)
    {
        BeforeOnWindowFocused(sender, args);
        OnWindowFocused(sender, args);
        AfterOnWindowFocused(sender, args);
        base.OnActivated(sender, args);
    }
    
    /// <summary>
    /// Called when the game window loses focus. This method is useful
    /// for pausing the game, stopping animations, or other actions when
    /// the game is no longer the active window.
    /// </summary>
    /// <param name="sender">
    /// The source of the event, typically the game window.
    /// </param>
    /// <param name="args">
    /// Arguments for the event, containing any additional info about the deactivation.
    /// </param>
    protected virtual void OnWindowClosed(object sender, EventArgs args) {}
    protected internal virtual void BeforeOnWindowClosed(object sender, EventArgs args) {}
    protected internal virtual void AfterOnWindowClosed(object sender, EventArgs args) {}
    protected sealed override void OnDeactivated(object sender, EventArgs args)
    {
        BeforeOnWindowClosed(sender, args);
        OnWindowClosed(sender, args);
        AfterOnWindowClosed(sender, args);
        base.OnDeactivated(sender, args);
    }


    #if DEBUG
    private readonly Stopwatch _totalRuntimeStopwatch = new();
    private int _updateCallCount;
    #endif
}
