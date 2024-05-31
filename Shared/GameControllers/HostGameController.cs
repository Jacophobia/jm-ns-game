using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Configuration;
using Shared.Controllables;
using Shared.Networking;
using Shared.Rendering;
using Shared.Singletons;

namespace Shared.GameControllers;

public abstract class GameController : Game
{
    // graphics
    private readonly GraphicsDeviceManager _graphicsDeviceManager;
    
    // multiplayer
    private readonly Server _server;
    private readonly Dictionary<Guid, IRenderer> _renderers;
    
    protected GameController(GameSettings settings)
    {
        // graphics
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
        _graphicsDeviceManager.IsFullScreen = settings.Fullscreen;

        // multiplayer
        _server = new Server(settings.HostPort);
        _renderers = new Dictionary<Guid, IRenderer>();
    }

    protected sealed override void Initialize()
    {
        _server.Start();
        
        base.Initialize();
    }

    protected sealed override void LoadContent()
    {
        TextureManager.Initialize(Content);
        LoadGraphicsPipeline();
        OnLoad();
        
        base.LoadContent();
    }

    private void LoadGraphicsPipeline()
    {
        var renderPipeline = new MonogameRenderPipeline
        {
            GraphicsDeviceManager = _graphicsDeviceManager,
            ContentManager = Content,
            GraphicsDevice = GraphicsDevice
        };

        // add a new renderer with an empty id for the local renderer
        var renderer = new LocalRenderer(renderPipeline);
        _renderers.Add(Guid.Empty, renderer);
    }
    protected abstract void OnLoad();

    protected sealed override void Update(GameTime gameTime)
    {
        while (_server.TryGetNewPlayer(out var newPlayerId))
        {
            var remoteRenderer = new RemoteRenderer(_server, newPlayerId);
            _renderers.Add(newPlayerId, remoteRenderer);
            var controller = new RemotePlayerController(_server, newPlayerId);
            OnPlayerConnected(newPlayerId, controller);
        }

        while (_server.TryGetDisconnectedPlayerId(out var disconnectedPlayerId))
        {
            _renderers.Remove(disconnectedPlayerId);
            OnPlayerDisconnected(disconnectedPlayerId);
        }
        
        OnUpdate(gameTime);
        
        base.Update(gameTime);
    }

    protected abstract void OnPlayerConnected(Guid newPlayerId, IController controller);
    protected abstract void OnPlayerDisconnected(Guid newPlayerId);
    protected abstract void OnUpdate(GameTime gameTime);

    protected sealed override void Draw(GameTime gameTime)
    {
        foreach (var (_, renderer) in _renderers)
        {
            renderer.Begin();
        }
        
        OnDraw(gameTime, _renderers);

        foreach (var (_, renderer) in _renderers)
        {
            renderer.End();
        }
        
        base.Draw(gameTime);
    }
    protected abstract void OnDraw(GameTime gameTime, Dictionary<Guid, IRenderer> renderers);

    protected sealed override void EndRun()
    {
        _server.Stop();
        
        base.EndRun();
    }

    protected override void Dispose(bool disposing)
    {
        _server?.Dispose();
        
        base.Dispose(disposing);
    }
}