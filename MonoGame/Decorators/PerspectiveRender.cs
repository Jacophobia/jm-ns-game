using System;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class PerspectiveRender : EntityDecorator
{
    private readonly bool _adjustScale;
    private float _scale;

    public PerspectiveRender(Entity @base, bool adjustScale) : base(@base)
    {
        _adjustScale = adjustScale;
    }

    protected override void BeforeDraw(IPlayer player)
    {
        var cameraDistance = Layer - player.Depth;

        if (cameraDistance < player.FocalLength) // TODO: decide if we want to keep this. It's nice to see the character, but it might not be the right aesthetic
        {
            var color = Color;
            color.A = (byte)(byte.MaxValue * (cameraDistance / player.FocalLength));
            Color = color;
        }

        if (cameraDistance == 0) cameraDistance = 0.001f;

        _scale = player.FocalLength / cameraDistance;
    }

    protected override void OnDraw(IPlayer player)
    {
        var drawnDestination = Destination;
        
        var offset = (player.Perspective.Center.ToVector2() - Position) * (1 - _scale);
        
        offset.Round();
        
        drawnDestination.Location += offset.ToPoint();
        
        // scale is focalLength divided by distance
        if (_adjustScale)
        {
            drawnDestination.Width = (int)Math.Round(drawnDestination.Width * Math.Abs(_scale));
            drawnDestination.Height = (int)Math.Round(drawnDestination.Height * Math.Abs(_scale));
        }
        
        player.Display(this, destination: drawnDestination);
    }
}