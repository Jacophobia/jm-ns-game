﻿using System;
using MonoGame.Controllers;
using MonoGame.Interfaces;
using MonoGame.Players;

namespace client.Controllers;
public class NonHostingClient : RemoteController
{
    private const string ServerIpAddress = "127.0.0.1"; // Replace with the server's IP
    private const int ServerPort = 12345; // Replace with the server's port
    private IPlayer _player;

    public NonHostingClient() : base(ServerIpAddress, ServerPort, false)
    {
        
    }

    protected override void OnLoadContent()
    {
        _player = new Remote(Renderer, WindowSize, NetworkClient);
    }

    protected override void OnUpdate(float deltaTime)
    {
        _player.Update(deltaTime);
    }

    protected override void OnDraw(float deltaTime)
    {
        // Retrieve renderable data from the network and render it
        _player.BeginDisplay();
        foreach (var renderable in NetworkClient.GetRenderableData())
        {
            renderable.Draw(_player);
        }
        _player.EndDisplay();
    }
}