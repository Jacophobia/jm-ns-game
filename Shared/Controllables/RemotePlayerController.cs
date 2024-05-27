using System;
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
}