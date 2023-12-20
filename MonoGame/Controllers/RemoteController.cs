using System;
using MonoGame.Networking;
using MonoGame.Output;

namespace MonoGame.Controllers;

public abstract class RemoteController : GameController
{
    protected readonly Client Client;

    protected RemoteController(string serverIpAddress, int serverPort, bool fullscreen = true) : base(fullscreen)
    {
        Client = new Client(serverIpAddress, serverPort);
    }

    protected internal override void BeforeOnInitialize()
    {
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content);
        
        base.BeforeOnInitialize();
    }

    protected internal override void BeforeOnBeginRun()
    {
        Client.Connect();
    }

    protected internal override void AfterOnExit(object sender, EventArgs args)
    {
        Client.Disconnect();
        
        base.AfterOnExit(sender, args);
    }

    protected internal override void AfterOnDispose(bool disposing)
    {
        if (disposing)
        {
            Client?.Dispose();
        }
        
        base.AfterOnDispose(disposing);
    }
}