using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Controllers;
using MonoGame.Input;

namespace client.Controllers;
public class NonHostingClient : RemoteController
{
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port

    public NonHostingClient() : base(ServerIpAddress, ServerPort, false) {}

    protected override void OnUpdate(float deltaTime, IList<Controls> controls)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        NetworkClient.SendControlData(controls.FirstOrDefault());
    }

    protected override void OnDraw(float deltaTime)
    {
        // Retrieve renderable data from the network and render it
        foreach (var renderable in NetworkClient.GetRenderableData())
        {
            Renderer.Draw(renderable);
        }
    }
}