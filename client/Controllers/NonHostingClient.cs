using System;
using Microsoft.Xna.Framework;
using MonoGame;
using MonoGame.Input;
using MonoGame.Networking;
using MonoGame.Output;

namespace client.Controllers;
public class NonHostingClient : GameController
{
    private NetworkClient _networkClient;
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port
    private Camera _camera;

    protected override void OnInitialize()
    {
        // Initialize NetworkClient
        _networkClient = new NetworkClient(ServerIpAddress, ServerPort);
    }

    protected override void OnLoadContent()
    {
        // Load any necessary content
    }

    protected override void OnBeginRun()
    {
        // Start the network client and its listening process
        _networkClient.Connect();
        _networkClient.StartListening();
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // Send control data
        _networkClient.SendControlData(controls);

        // Additional update logic
    }

    protected override void OnBeginDraw()
    {
        // Before drawing
    }

    protected override void OnDraw(GameTime gameTime)
    {
        // Retrieve renderable data from the network and render it
        foreach (var renderable in _networkClient.GetRenderableData(gameTime.TotalGameTime.Milliseconds))
        {
            Renderer.Render(renderable, _camera);
        }
    }

    protected override void OnEndDraw()
    {
        // After drawing
    }

    protected override void OnEndRun()
    {
        // Clean up on game end
    }

    protected override void OnUnloadContent()
    {
        // Unload any game content
    }

    protected override void OnExit(object sender, EventArgs args)
    {
        // Handle game exit events
    }

    protected override void OnDispose(bool disposing)
    {
        // Dispose resources
        if (disposing)
        {
            _networkClient?.Dispose();
        }
    }

    protected override void OnWindowFocused(object sender, EventArgs args)
    {
        // Handle window focus events
    }

    protected override void OnWindowClosed(object sender, EventArgs args)
    {
        // Handle window close events
    }
}