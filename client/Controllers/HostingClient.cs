using System;
using System.Collections.Generic;
using client.Entities;
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
    private const int NumLayers = 1;
    private const int StartingLayer = 0;
    private const int LayerDepth = 1;
    private const int BallsPerLayer = 1;

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
            var color = new Color(random.Next(50), random.Next(50), random.Next(255));
            for (var j = 0; j < BallsPerLayer; j++)
            {
                var ballPosition =
                    new Vector2(random.Next(2560 - maxBallSize), random.Next(1440 - maxBallSize));
                var size = random.Next(minBallSize, maxBallSize);

                var entity = Characters.Default(
                    position: ballPosition,
                    velocity: new Vector2(GetNonZeroRandom(-2, 2), GetNonZeroRandom(-2, 2)) * random.Next(1, 5) * 60,
                    scale: 2,
                    depth: i * LayerDepth,
                    color: color);
                
                switch (i)
                {
                    case 0 when j is 0:
                    {
                        var player = new Host(new Camera(followSpeed: 1f, offset: new Vector3(0, 100, -10)), Renderer);
                        
                        entity.SetColor(Color.Red);
                        entity.SetVelocity(Vector2.Zero);
                        entity.Add<Jump>(player, 5f, 0.75f, 0.5f, 3.5f);
                        entity.Add<LRMovement>(player, 0.75f);

                        var mainEntity = entity.Build();
                        
                        player.Follow(mainEntity); 
                        
                        _spatialPartition.Add(player);
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
                    .Add<Static>()
                    .Add<Collision>()
                    .Add<Rectangular>()
                    .Add<PerspectiveRender>(true)
                    .Build());
            }
        foreach (var entity in background)
            _spatialPartition.Add(entity);
    }

    protected override void BeforeOnUpdate(float deltaTime)
    {
        if (!Server.TryGetNewPlayer(out var newPlayer))
            return;

        var newCharacterBuilder = _playerEntities.Pop(); // TODO: need to handle the case where there aren't enough characters available

        newCharacterBuilder.SetVelocity(Vector2.Zero);
        newCharacterBuilder.Add<Jump>(newPlayer, 5f, 0.75f, 0.5f, 3.5f);
        newCharacterBuilder.Add<LRMovement>(newPlayer, 0.75f);

        var newCharacter = newCharacterBuilder.Build();

        newPlayer.Follow(newCharacter);
        
        _spatialPartition.Add(newCharacter);
        _spatialPartition.Add(newPlayer);
    }

    protected override void OnUpdate(float deltaTime)
    {
        _spatialPartition.Update(deltaTime);
    }

    protected override void AfterOnUpdate(float deltaTime)
    {
        if (!Server.TryGetDisconnectedPlayerId(out var playerId))
            return;

        _spatialPartition.Remove(playerId);
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
