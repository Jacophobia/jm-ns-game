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
        var cameraDistance = Depth - player.Depth;

        // if (cameraDistance < 10) // TODO: decide if we want to keep this. It's nice to see the character, but it might not be the right aesthetic
        // {
        //     var color = Color;
        //     color.A = (byte)(byte.MaxValue * (cameraDistance / 10f));
        //     Color = color;
        // }

        if (cameraDistance == 0) cameraDistance = 0.001f;

        _scale = MathF.Abs(player.Depth / cameraDistance);
    }

    protected override void OnDraw(IPlayer player)
    {
        var drawnDestination = Destination;
        
        var offset = (player.Perspective.Center.ToVector2() - Position) * (1 - _scale);
        
        drawnDestination.X += (int)Math.Round(offset.X);
        drawnDestination.Y += (int)Math.Round(offset.Y);
        
        // scale is focalLength divided by distance
        if (_adjustScale)
        {
            drawnDestination.Width = (int)Math.Round(drawnDestination.Width * Math.Abs(_scale));
            drawnDestination.Height = (int)Math.Round(drawnDestination.Height * Math.Abs(_scale));
        }
        
        player.Display(this, destination: drawnDestination);
    }
}