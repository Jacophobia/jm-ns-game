using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Networking;
using MonoGame.Output;

namespace MonoGame.Controllers;

public abstract class HostController : GameController
{
    private readonly NetworkClient _networkClient;

    protected HostController(int serverPort, bool fullscreen = true) : base(fullscreen)
    {
        _networkClient = new NetworkClient(serverPort);
    }

    protected internal override void BeforeOnInitialize()
    {
        Renderer = new Renderer(GraphicsDevice, SpriteBatch, Content, _networkClient);
        _networkClient.StartListening();

        base.BeforeOnInitialize();
    }

    protected internal override void BeforeOnUpdate(GameTime gameTime, IList<Controls> controls)
    {
        controls.Add(_networkClient.GetControlData());
        
        base.BeforeOnUpdate(gameTime, controls);
    }

    protected internal override void AfterOnExit(object sender, EventArgs args)
    {
        _networkClient.Disconnect();
        
        base.AfterOnExit(sender, args);
    }

    protected internal override void AfterOnDispose(bool disposing)
    {
        if (disposing)
        {
            _networkClient?.Dispose();
        }
        
        base.AfterOnDispose(disposing);
    }
}