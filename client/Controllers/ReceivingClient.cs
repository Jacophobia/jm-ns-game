using MonoGame.Controllers;
using MonoGame.Interfaces;
using MonoGame.Players;

namespace client.Controllers;
public class ReceivingClient : RemoteController
{
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port
    private IPlayer _player;

    public ReceivingClient() : base(ServerIpAddress, ServerPort, false)
    {
        
    }

    protected override void OnLoadContent()
    {
        _player = new Remote(Renderer, WindowSize, Client);
    }

    protected override void OnUpdate(float deltaTime)
    {
        _player.Update(deltaTime);
    }

    protected override void OnDraw(float deltaTime)
    {
        // Retrieve renderable data from the network and render it
        _player.BeginDisplay();
        foreach (var renderable in Client.GetRenderableData())
        {
            renderable.Render(_player);
        }
        _player.EndDisplay();
    }
}