using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame;
using MonoGame.Decorators;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Networking;
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.SpatialPartitions; // Assuming this is where IRenderable is located

namespace client.Controllers;

public class HostingClient : GameController
{
    private NetworkClient _networkClient;
    private const string ServerIpAddress = "127.0.0.1"; // Replace with actual server IP
    private const int ServerPort = 12345; // Replace with the actual port
    private ISpatialPartition<Entity> _spatialPartition;
    private Camera _camera;

    protected override void OnInitialize()
    {
        // Initialize NetworkClient
        _networkClient = new NetworkClient(ServerIpAddress, ServerPort);
        _spatialPartition = new SpatialGrid<Entity>();
    }

    private static int GetNonZeroRandom(int min, int max)
    {
        var random = new Random();
        var randomNum = 0;
        while (randomNum == 0) randomNum = random.Next(min, max);
        return randomNum;
    }

    protected override void OnLoadContent()
    {
        var random = new Random();
        const int minBallSize = 1;
        const int maxBallSize = 5;
        const int numBalls = 50;

        for (var i = -1; i < numBalls; i++)
        {
            var color = new Color(random.Next(200), random.Next(255), random.Next(255));
            for (var j = 0; j < 10; j++)
            {
                var ballPosition =
                    new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
                var size = random.Next(minBallSize, maxBallSize);

                var entityBuilder = new EntityBuilder(
                        "Test/ball",
                        ballPosition,
                        new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) *
                        (1f / 0.016f),
                        50,
                        50)
                    .SetDepth(i * 5)
                    .SetColor(color)
                    .AddDecorator<Inertia>()
                    .AddDecorator<CircularCollision>()
                    .AddDecorator<Bound>(new Rectangle(0, 0, 2560, 1440))
                    .AddDecorator<PerspectiveRender>(true, -10);

                if (i == 0)
                {
                    entityBuilder.SetColor(Color.Red);
                    entityBuilder.SetDepth(0);
                    var entity = entityBuilder.Build();
                    _camera = new Camera(entity, 1, Vector3.Up * 100);
                    _spatialPartition.Add(entity);
                    continue;
                }

                _spatialPartition.Add(entityBuilder.Build());
            }
        }
    }

    protected override void OnBeginRun()
    {
        // Start the network client and its listening process
        _networkClient.Connect();
        _networkClient.StartListening();
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // Receive control data from the network
        foreach (var control in _networkClient.GetControlData(gameTime.TotalGameTime.Milliseconds))
        {
            // Process received control data
            ProcessControlData(control);
        }
    }

    protected override void OnBeginDraw()
    {
        // Before drawing
    }

    protected override void OnDraw(GameTime gameTime)
    {
        // Draw game elements

        // Send renderable data
        foreach (var renderable in GetRenderables())
        {
            _networkClient.SendRenderableData(renderable);
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

    private void ProcessControlData(Controls control)
    {
        // Implement control processing logic
    }

    private IEnumerable<IRenderable> GetRenderables()
    {
        return _spatialPartition;
    }
}
