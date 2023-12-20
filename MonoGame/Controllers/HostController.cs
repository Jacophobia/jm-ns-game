using System;
using MonoGame.Networking;
using MonoGame.Output;

namespace MonoGame.Controllers;

public abstract class HostController : GameController
{
    protected readonly Server Server;

    protected HostController(int serverPort, bool fullscreen = true) : base(fullscreen)
    {
        Server = new Server(serverPort);
    }

    protected internal override void BeforeOnInitialize()
    {
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content);
        Server.Start();

        base.BeforeOnInitialize();
    }

    protected internal override void AfterOnExit(object sender, EventArgs args)
    {
        Server.Stop();
        
        base.AfterOnExit(sender, args);
    }

    protected internal override void AfterOnDispose(bool disposing)
    {
        if (disposing)
        {
            Server?.Dispose();
        }
        
        base.AfterOnDispose(disposing);
    }
}