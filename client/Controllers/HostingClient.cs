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
using MonoGame.Players;

namespace client.Controllers;

public class HostingClient : HostController
{
    private const int ServerPort = 12345;
    private const int NumLayers = 20;
    private const int StartingLayer = -1;
    private const int LayerDepth = 5;
    private const int BallsPerLayer = 50;
    
    private ISpatialPartition<Entity> _spatialPartition;

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
        const int minBallSize = 1;
        const int maxBallSize = 100;

        for (var i = StartingLayer; i < NumLayers; i++)
        {
            var color = new Color(random.Next(150), random.Next(255), random.Next(255));
            for (var j = 0; j < BallsPerLayer; j++)
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
                    .SetDepth(i * LayerDepth)
                    .SetColor(color)
                    .AddDecorator<Friction>(1f)
                    .AddDecorator<RemoveJitter>(0.125f)
                    .AddDecorator<Inertia>()
                    .AddDecorator<Collision>()
                    // .AddDecorator<Friction>(200f)
                    .AddDecorator<BasicMovement>()
                    // .AddDecorator<Rectangular>()
                    .AddDecorator<Circular>()
                    // .AddDecorator<Gravity>()
                    // .AddDecorator<Bound>(WindowSize)
                    // .AddDecorator<Bound>(new Rectangle(-2560 / 2, -1440 / 2, 2560 * 2, 1440 * 2))
                    .AddDecorator<PerspectiveRender>(true);
                
                switch (i)
                {
                    case 0 when j is 0:
                    {
                        entity.SetColor(Color.Red);
                        var mainEntity = entity.Build();
                        Players.Add(new Host(new Camera(mainEntity, 1, Vector3.Up * 100), Renderer));
                        _spatialPartition.Add(mainEntity);
                        break;
                    }
                    case 1 when j is 0:
                    {
                        var otherEntity = entity.Build();
                        Players.Add(new Remote(new Camera(otherEntity, 1, new Vector3(0f, -100f, -15f)), NetworkClient));
                        _spatialPartition.Add(otherEntity);
                        break;
                    }
                    default:
                        _spatialPartition.Add(entity.Build());
                        break;
                }
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
                    .SetDepth(i * LayerDepth)
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

    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        _spatialPartition.Update(deltaTime, controls);
    }

    protected override void OnDraw(float deltaTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _spatialPartition.Draw(Players, deltaTime);
    }

    protected override void OnDispose(bool disposing)
    {
        _spatialPartition?.Dispose();
    }
}
