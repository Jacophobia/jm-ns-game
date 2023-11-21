using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Input;
using MonoGame.Output;

namespace MonoGame;

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
public abstract class GameController : Game
{
    protected Renderer Renderer;

    protected Rectangle WindowSize
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
    private Listener _inputListener;
    private SpriteBatch _spriteBatch;

    protected GameController(bool fullscreen = true)
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Window.AllowUserResizing = true;
        IsMouseVisible = true;

        var windowSize = WindowSize;
        
        _graphicsDeviceManager.PreferredBackBufferWidth = windowSize.Width;
        _graphicsDeviceManager.PreferredBackBufferHeight = windowSize.Height;
        // Set fullscreen mode
        _graphicsDeviceManager.IsFullScreen = fullscreen;
    }
    
    protected sealed override void Initialize()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _inputListener = new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down }
        });
        Renderer = new Renderer(GraphicsDevice, _spriteBatch, Content);
        
        OnInitialize();
        
        base.Initialize();
    }

    /// <summary>
    /// Initializes game component. Called once before the game loop
    /// starts and after the graphics device is initialized.
    /// </summary>
    protected abstract void OnInitialize();

    protected sealed override void LoadContent()
    {
        OnLoadContent();
        
        base.LoadContent();
    }

    /// <summary>
    /// Called when the game should load its content.
    /// </summary>
    protected abstract void OnLoadContent();

    protected sealed override void BeginRun()
    {
        OnBeginRun();
        base.BeginRun();
    }

    /// <summary>
    /// Called once before the game loop begins. Useful for any one-time
    /// setup.
    /// </summary>
    protected abstract void OnBeginRun();

    protected sealed override void Update(GameTime gameTime)
    {
        OnUpdate(gameTime, _inputListener.GetInputState());
        base.Update(gameTime);
    }

    /// <summary>
    /// Called each frame to update the game logic.
    /// </summary>
    /// <param name="gameTime">
    ///     Snapshot of the game's timing state.
    /// </param>
    /// <param name="controls"></param>
    protected abstract void OnUpdate(GameTime gameTime, Controls controls);

    protected sealed override bool BeginDraw()
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        OnBeginDraw();
        return base.BeginDraw();
    }

    /// <summary>
    /// Called before drawing the frame. Allows any necessary pre-draw
    /// operations.
    /// </summary>
    protected abstract void OnBeginDraw();

    protected sealed override void Draw(GameTime gameTime)
    {
        OnDraw(gameTime);
        base.Draw(gameTime);
    }

    /// <summary>
    /// Called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">
    /// Snapshot of the game's timing state.
    /// </param>
    protected abstract void OnDraw(GameTime gameTime);

    protected sealed override void EndDraw()
    {
        OnEndDraw();
        _spriteBatch.End();
        base.EndDraw();
    }

    /// <summary>
    /// Called after drawing the frame. Allows any necessary post-draw
    /// operations.
    /// </summary>
    protected abstract void OnEndDraw();

    protected sealed override void EndRun()
    {
        OnEndRun();
        base.EndRun();
    }

    /// <summary>
    /// Called once after the game loop ends. Useful for any cleanup.
    /// </summary>
    protected abstract void OnEndRun();

    protected sealed override void UnloadContent()
    {
        OnUnloadContent();
        base.UnloadContent();
    }

    /// <summary>
    /// Called when the game should unload its content.
    /// </summary>
    protected abstract void OnUnloadContent();

    protected sealed override void OnExiting(object sender, EventArgs args)
    {
        OnExit(sender, args);
        base.OnExiting(sender, args);
    }

    /// <summary>
    /// Called when the game is exiting. Triggers the OnExit method for
    /// any necessary cleanup.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">Arguments for the event.</param>
    protected abstract void OnExit(object sender, EventArgs args);

    protected sealed override void Dispose(bool disposing)
    {
        OnDispose(disposing);
        base.Dispose(disposing);
    }

    /// <summary>
    /// Disposes of the game's resources. Called when the game is being
    /// destroyed.
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether the method call comes from a Dispose method
    /// (its value is true) or from a finalizer (its value is false).
    /// </param>
    protected abstract void OnDispose(bool disposing);

    protected sealed override void OnActivated(object sender, EventArgs args)
    {
        OnWindowFocused(sender, args);
        base.OnActivated(sender, args);
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
    protected abstract void OnWindowFocused(object sender, EventArgs args);

    protected sealed override void OnDeactivated(object sender, EventArgs args)
    {
        OnWindowClosed(sender, args);
        base.OnDeactivated(sender, args);
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
    protected abstract void OnWindowClosed(object sender, EventArgs args);
}
