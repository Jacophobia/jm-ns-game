using System;
using System.Collections.Generic;
using client.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Controllers;
using MonoGame.DataStructures;
using MonoGame.Decorators;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace client.Controllers;

public class HostingClient : HostController
{
    private const int ServerPort = 12345;
    private const int NumLayers = 50;
    private const int StartingLayer = -1;
    private ISpatialPartition<Entity> _spatialPartition;
    private Camera _camera1;
    private Camera _camera2;

    public HostingClient() : base(ServerPort, fullscreen: false)
    {
        
    }

    protected override void OnInitialize()
    {
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
        SetBackground();
        
        var random = new Random();
        const int minBallSize = 100;
        const int maxBallSize = 100;
        const int ballsPerLayer = 10;

        for (var i = StartingLayer; i < NumLayers; i++)
        {
            var color = new Color(random.Next(200), random.Next(255), random.Next(255));
            for (var j = 0; j < ballsPerLayer; j++)
            {
                var ballPosition =
                    new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
                var size = random.Next(minBallSize, maxBallSize);

                var entity = new EntityBuilder(
                        "Test/ball",
                        ballPosition,
                        new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) *
                        (1f / 0.016f),
                        size,
                        size)
                    .SetDepth(i * 5)
                    .SetColor(color)
                    .AddDecorator<Inertia>()
                    .AddDecorator<Collision>()
                    .AddDecorator<Circular>()
                    .AddDecorator<Bound>(new Rectangle(-2560 / 2, -1440 / 2, 2560 * 2, 1440 * 2))
                    .AddDecorator<PerspectiveRender>(true)
                    .Build();
                
                if (i is 0 && j is 0)
                {
                    _camera1 = new Camera(entity, 1, Vector3.Up * 100, 0);
                    _camera2 = new Camera(entity, 1, Vector3.Up * 100, 1);
                }
                
                _spatialPartition.Add(entity);
            }
        }
    }

    private void SetBackground()
    {
        var backgroundTexture = Content.Load<Texture2D>("Test/background");
        // var backgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
        // backgroundTexture.SetData(new[] { Color.White }); // Set the pixel to black
        // backgroundTexture.Name = "Test/background";
        var background = new List<Entity>();
        
        foreach (var side in WindowSize.GetOutline(200).GetSides())
            for (var i = StartingLayer; i < NumLayers; i++)
            {
                background.Add(new EntityBuilder(
                        backgroundTexture,
                        new Vector2(side.X, side.Y),
                        Vector2.Zero,
                        side.Width,
                        side.Height)
                    .SetDepth(5 * i)
                    .SetStatic(true)
                    .SetColor(Color.White)
                    .AddDecorator<Static>()
                    .AddDecorator<Collision>()
                    .AddDecorator<Rectangular>()
                    .AddDecorator<PerspectiveRender>(true)
                    .Build());
            }
        foreach (var entity in background)
            _spatialPartition.Add(entity);
    }

    protected override void OnUpdate(float deltaTime, IList<Controls> controls)
    {
        _spatialPartition.Update(deltaTime, controls);
        _camera1.Update(deltaTime, controls);
        _camera2.Update(deltaTime, controls);
    }

    protected override void OnDraw(float deltaTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _spatialPartition.Draw(Renderer, new []{ _camera1, _camera2 }, deltaTime);
    }

    protected override void OnDispose(bool disposing)
    {
        _spatialPartition?.Dispose();
    }
}
