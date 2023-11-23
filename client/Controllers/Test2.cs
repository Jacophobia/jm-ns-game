using System;
using System.Collections.Generic;
using client.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.DataStructures;
using MonoGame.Decorators;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace client.Controllers;

public class Test2 : GameController
{
    private readonly ISpatialPartition<Entity> _spatialGrid;
    private List<Entity> _background;
    private Camera _camera;

    public Test2()
    {
        _spatialGrid = new SpatialGrid<Entity>();
    }

    protected override void OnInitialize()
    {
        // nothing to implement
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
                    _spatialGrid.Add(entity);
                    continue;
                }

                _spatialGrid.Add(entityBuilder.Build());
            }
        }
    }

    private void SetBackground()
    {
        var backgroundTexture = new Texture2D(GraphicsDevice, 1, 1);
        backgroundTexture.SetData(new[] { Color.White }); // Set the pixel to black
        backgroundTexture.Name = "Background";
        _background = new List<Entity>();
        foreach (var side in WindowSize.GetOutline(50).GetSides())
            for (var i = -1; i < 50; i++)
                _background.Add(new EntityBuilder(
                        backgroundTexture,
                        new Vector2(side.X, side.Y),
                        Vector2.Zero,
                        side.Width,
                        side.Height)
                    .SetDepth(5 * i)
                    .SetStatic(true)
                    .SetColor(Color.White)
                    .AddDecorator<PerspectiveRender>(true, -10)
                    .Build());
    }

    protected override void OnBeginRun()
    {
        // nothing to implement
    }

    protected override void OnUpdate(GameTime gameTime, Controls[] controls)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        // This will call Update on each Ball
        _spatialGrid.Update(gameTime, controls[0]); 
        _camera.Update(gameTime, controls[0]);
    }

    protected override void OnBeginDraw()
    {
        // nothing to implement
    }

    protected override void OnDraw(GameTime gameTime)
    {
        foreach (var side in _background) side.Draw(Renderer, _camera);
        _spatialGrid.Draw(Renderer, _camera, gameTime);
    }

    protected override void OnEndDraw()
    {
        // nothing to implement
    }

    protected override void OnEndRun()
    {
        // nothing to implement
    }

    protected override void OnUnloadContent()
    {
        // nothing to implement
    }

    protected override void OnExit(object sender, EventArgs args)
    {
        // nothing to implement
    }

    protected override void OnDispose(bool disposing)
    {
        // nothing to implement
    }

    protected override void OnWindowFocused(object sender, EventArgs args)
    {
        // nothing to implement
    }

    protected override void OnWindowClosed(object sender, EventArgs args)
    {
        // nothing to implement
    }
}