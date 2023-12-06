using MonoGame.Controllers;
using MonoGame.Input;
using MonoGame.Players;

namespace client.Controllers;
public class NonHostingClient : RemoteController
{
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port

    public NonHostingClient() : base(ServerIpAddress, ServerPort, false)
    {
        
    }

    protected override void OnLoadContent()
    {
        Players.Add(new Basic(WindowSize, -10, Renderer));
    }

    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        NetworkClient.SendControlData(controls);
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