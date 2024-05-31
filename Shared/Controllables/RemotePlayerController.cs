using System;
using Microsoft.Xna.Framework;
using Shared.Networking;

namespace Shared.Controllables;

public class RemotePlayerController : IController
{
    private readonly Guid _playerId;
    private readonly Server _server;

    public RemotePlayerController(Server server, Guid playerId)
    {
        _playerId = playerId;
        _server = server;
    }

    public Controls Controls => _server.GetControls(_playerId);
    public Vector2 LeftJoystick => throw new System.NotImplementedException();
    public Vector2 RightJoystick => throw new System.NotImplementedException();
}