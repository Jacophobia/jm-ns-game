using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame;
using MonoGame.DataStructures;
using MonoGame.Decorators;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Networking;
using MonoGame.Interfaces;
using MonoGame.Output;

// Assuming this is where IRenderable is located

namespace client.Controllers;

public class HostingClient : GameController
{
    private ISpatialPartition<Entity> _spatialPartition;
    private Camera _camera1;
    private Camera _camera2;

    protected override void OnInitialize()
    {
        // Initialize NetworkClient
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

                switch (i)
                {
                    case 0:
                    {
                        entityBuilder.SetColor(Color.Red);
                        entityBuilder.SetDepth(0);
                        var entity = entityBuilder.Build();
                        _camera1 = new Camera(entity, 1, Vector3.Up * 100, i);
                        _spatialPartition.Add(entity);
                        continue;
                    }
                    case 1:
                    {
                        entityBuilder.SetColor(Color.Red);
                        entityBuilder.SetDepth(0);
                        var entity = entityBuilder.Build();
                        _camera2 = new Camera(entity, 1, Vector3.Up * 100, i);
                        _spatialPartition.Add(entity);
                        continue;
                    }
                    default:
                        _spatialPartition.Add(entityBuilder.Build());
                        break;
                }
            }
        }
    }

    protected override void OnBeginRun()
    {
    }

    protected override void OnUpdate(GameTime gameTime, Controls[] controls)
    {
        
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
