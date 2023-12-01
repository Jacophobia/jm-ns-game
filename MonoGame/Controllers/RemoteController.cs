using System;
using System.Collections.Generic;
using MonoGame.Input;
using MonoGame.Networking;
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
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content, NetworkClient);
        
        base.BeforeOnInitialize();
    }

    protected internal override void BeforeOnBeginRun()
    {
        // Start the network client and its listening process
        NetworkClient.Connect();
        NetworkClient.StartListening();
    }

    protected internal override void BeforeOnUpdate(float deltaTime, IList<Controls> controls)
    {
        controls.Add(NetworkClient.GetControlData());
        
        base.BeforeOnUpdate(deltaTime, controls);
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