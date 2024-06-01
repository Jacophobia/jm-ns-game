using Microsoft.Xna.Framework;
using Shared.Configuration;
using Shared.Controllables;
using Shared.Networking;
using Shared.Rendering;
using Shared.Singletons;

namespace Shared.GameControllers;

public sealed class RemoteGameController : Game
{
    private readonly Client _client;
    private readonly IController _controller;
    private  TextureManager _textureManager;
    private IRenderer _renderer;
    
    
    public RemoteGameController(GameSettings settings)
    {
        _client = new Client(settings.HostIp, settings.HostPort);
        _controller = new PlayerController(settings);
    }

    protected override void LoadContent()
    {
        TextureManager.Initialize(Content);
        _textureManager = TextureManager.GetInstance();
        
        _renderer = new LocalRenderer(GraphicsDevice, Content);
        
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _client.Send(_controller.Controls);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _renderer.Begin();

        var renderables = _client.GetRenderables();

        foreach (var renderable in renderables)
        {
            _renderer.Render(
                _textureManager[renderable.TextureName], 
                renderable.Destination,
                renderable.Source,
                renderable.Color,
                renderable.Rotation,
                renderable.Origin,
                renderable.Effect,
                renderable.Depth
            );
        }
        
        _renderer.End();
        
        base.Draw(gameTime);
    }

    protected override void BeginRun()
    {
        _client.Connect();
        
        base.BeginRun();
    }

    protected override void EndRun()
    {
        _client.Disconnect();
        
        base.EndRun();
    }

    protected override void Dispose(bool disposing)
    {
        _client?.Dispose();
        
        base.Dispose(disposing);
    }
}