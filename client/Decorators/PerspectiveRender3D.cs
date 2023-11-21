using System;
using System.Collections.Generic;
using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpatialPartition.Interfaces;

namespace client.Decorators;

internal class RenderDetails
{
    internal Texture2D Texture { get; init; }
    internal int Depth { get; init; }
    internal int Width { get; init; }
    internal int Height { get; init; }
    internal float Scale { get; init; }
}

public class PerspectiveRender3D : EntityDecorator
{
    private readonly IList<RenderDetails> _layers;

    public PerspectiveRender3D(Entity @base, bool adjustScale, float perspectiveDepth, GraphicsDevice graphicsDevice) : base(@base)
    {
        _layers = new List<RenderDetails>();
        var width = @base.Texture.Width;
        var height = @base.Texture.Height;
        var data = new Color[width * height];   
        @base.Texture.GetData(data);
        for (var row = 0; row < height; row++)
        for (var column = 0; column < width; column++)
        {
            var layerData = new Color[width * width];
            
            for (var layerRow = 0; layerRow < width; layerRow++)
            {
                layerData.Set(column, layerRow, width, data.Get(column, row, width));
            }
            
            var layerDepth = Depth - (height + row);
                
            var cameraDistance = layerDepth - perspectiveDepth;

            if (cameraDistance == 0) cameraDistance = 0.001f;

            var scale = MathF.Abs(perspectiveDepth / cameraDistance);
            
            var layer = new Texture2D(graphicsDevice, width, width);
            layer.SetData(layerData);
            var layerWidth = (int)Math.Round(width * (adjustScale ? Math.Abs(scale) : 1f));
            _layers.Add(new RenderDetails 
            { 
                Texture = layer,
                Depth = layerDepth,
                Width = layerWidth,
                Height = layerWidth,
                Scale = 1 - scale
            });
        }
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera camera)
    {
        foreach (var layer in _layers)
        {
            var actualPosition = Position;
            Position += (camera.View.Center.ToVector2() - Position) * layer.Scale;
            var layerDestination = Destination;
            layerDestination.Width = layer.Width;
            layerDestination.Height = layer.Height;
            renderer.Render(this, camera, texture: layer.Texture, destination: layerDestination, depth: layer.Depth);
            Position = actualPosition;
        }
    }
}