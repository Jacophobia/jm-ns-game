using System;
using MonoGame.Output;

namespace MonoGame.Controllers;

public abstract class HostController : GameController
{
    protected readonly NetworkHost NetworkClient;

    protected HostController(int serverPort, bool fullscreen = true) : base(fullscreen)
    {
        NetworkClient = new NetworkHost(serverPort);
    }

    protected internal override void BeforeOnInitialize()
    {
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content);
        NetworkClient.StartListening();

        base.BeforeOnInitialize();
    }

    protected internal override void AfterOnExit(object sender, EventArgs args)
    {
        NetworkClient.Disconnect();
        
        base.AfterOnExit(sender, args);
    }

    protected internal override void AfterOnDispose(bool disposing)
    {
        if (disposing)
        {
            NetworkClient?.Dispose();
        }
        
        base.AfterOnDispose(disposing);
    }
}