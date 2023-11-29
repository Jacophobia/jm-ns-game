using System;
using MonoGame.Entities;
using MonoGame.Output;

namespace MonoGame.Decorators;

public class PerspectiveRender : EntityDecorator
{
    private readonly bool _adjustScale;
    private float _scale;

    public PerspectiveRender(Entity @base, bool adjustScale) : base(@base)
    {
        _adjustScale = adjustScale;
    }

    protected override void BeforeDraw(Renderer renderer, Camera camera)
    {
        var cameraDistance = Depth - camera.Position.Z;

        if (cameraDistance == 0) cameraDistance = 0.001f;

        _scale = MathF.Abs(camera.Position.Z / cameraDistance);
    }

    protected override void OnDraw(Renderer renderer, Camera camera)
    {
        var drawnDestination = Destination;
        
        var offset = (camera.View.Center.ToVector2() - Position) * (1 - _scale);
        
        drawnDestination.X += (int)Math.Round(offset.X);
        drawnDestination.Y += (int)Math.Round(offset.Y);
        
        // scale is focalLength divided by distance
        if (_adjustScale)
        {
            drawnDestination.Width = (int)Math.Round(drawnDestination.Width * Math.Abs(_scale));
            drawnDestination.Height = (int)Math.Round(drawnDestination.Height * Math.Abs(_scale));
        }
        
        renderer.Render(this, camera, destination: drawnDestination);
    }
}