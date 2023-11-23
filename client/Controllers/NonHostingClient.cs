using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.Input;

namespace client.Controllers;
public class NonHostingClient : GameController
{
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port

    public NonHostingClient() : base(ServerIpAddress, ServerPort, false)
    {
        
    }

    protected override void OnInitialize()
    {
        // Initialize NetworkClient
    }

    protected override void OnLoadContent()
    {
        // Load any necessary content
    }

    protected override void OnBeginRun()
    {
        // Start the network client and its listening process
        NetworkClient.Connect();
        NetworkClient.StartListening();
    }

    protected override void OnUpdate(GameTime gameTime, Controls[] controls)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // Send control data
        NetworkClient.SendControlData(controls[1]);

        // Additional update logic
    }

    protected override void OnBeginDraw()
    {
        // Before drawing
    }

    protected override void OnDraw(GameTime gameTime)
    {
        // Retrieve renderable data from the network and render it
        foreach (var renderable in NetworkClient.GetRenderableData((long)gameTime.TotalGameTime.TotalMilliseconds))
        {
            Renderer.Render(renderable);
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
            NetworkClient?.Dispose();
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