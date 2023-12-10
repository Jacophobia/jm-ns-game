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
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.Players;

namespace client.Controllers;

public class HostingClient : HostController
{
    private const int ServerPort = 12345;
    private const int NumLayers = 50;
    private const int StartingLayer = 0;
    private const int LayerDepth = 1;
    private const int BallsPerLayer = 10;

    private ISpatialPartition<Entity> _spatialPartition;
    private Stack<EntityBuilder> _playerEntities;

    public HostingClient() : base(ServerPort, fullscreen: false)
    {
        
    }

    protected override void OnInitialize()
    { 
        _spatialPartition = new SpatialGrid<Entity>();
        _playerEntities = new Stack<EntityBuilder>();
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
                        new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) * 60,
                        size,
                        size)
                    .SetDepth(i * LayerDepth)
                    .SetColor(color)
                    .AddDecorator<Friction>(0.01f)
                    .AddDecorator<RemoveJitter>(0.125f)
                    .AddDecorator<Inertia>()
                    .AddDecorator<Collision>()
                    // .AddDecorator<Drag>(20f)
                    // .AddDecorator<BasicMovement>()
                    .AddDecorator<Rectangular>()
                    // .AddDecorator<Circular>()
                    .AddDecorator<Gravity>()
                    // .AddDecorator<Bound>(new Rectangle(-2560 / 2, -1440 / 2, 2560 * 2, 1440 * 2))
                    .AddDecorator<PerspectiveRender>(true);
                
                switch (i)
                {
                    case 0 when j is 0:
                    {
                        entity.SetColor(Color.Red);
                        
                        var mainEntity = entity.Build();
                        _spatialPartition.Add(new Host(new Camera(mainEntity, 1f, new Vector3(0, 100, -20)), Renderer));
                        _spatialPartition.Add(mainEntity);
                        break;
                    }
                    case 0:
                    {
                        entity.SetColor(Color.Red);
                        _spatialPartition.Add(entity.Build());
                        break;
                    }
                    case 1:
                    {
                        entity.SetColor(Color.Blue);
                        _playerEntities.Push(entity);
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

    protected override void BeforeOnUpdate(float deltaTime)
    {
        if (!NetworkClient.TryGetNewPlayer(out var newPlayer))
            return;

        var newCharacterBuilder = _playerEntities.Pop(); // TODO: need to handle the case where there aren't enough characters available

        newCharacterBuilder.AddDecorator<BasicMovement>(newPlayer);

        var newCharacter = newCharacterBuilder.Build();

        newPlayer.Follow(newCharacter);
        
        _spatialPartition.Add(newCharacter);
        _spatialPartition.Add(newPlayer);
    }

    protected override void OnUpdate(float deltaTime)
    {
        _spatialPartition.Update(deltaTime);
    }

    protected override void OnDraw(float deltaTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _spatialPartition.Draw(deltaTime);
    }

    protected override void OnDispose(bool disposing)
    {
        _spatialPartition?.Dispose();
    }
}
