using System;
using MonoGame.Output;

namespace MonoGame.Controllers;

public abstract class RemoteController : GameController
{
    protected readonly NetworkClient NetworkClient;

    protected RemoteController(string serverIpAddress, int serverPort, bool fullscreen = true) : base(fullscreen)
    {
        NetworkClient = new NetworkClient(serverPort, serverIpAddress);
    }

    protected internal override void BeforeOnInitialize()
    {
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content);
        
        base.BeforeOnInitialize();
    }

    protected internal override void BeforeOnBeginRun()
    {
        // Start the network client and its listening process
        NetworkClient.Connect();
        NetworkClient.StartListening();
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