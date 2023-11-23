using System;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Decorators;

public class PerspectiveRender : EntityDecorator
{
    private readonly float _scale;

    // ReSharper disable once IntroduceOptionalParameters.Global
    public PerspectiveRender(Entity @base) : this(@base, false, -10f)
    {
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public PerspectiveRender(Entity @base, bool adjustScale, float perspectiveDepth) : base(@base)
    {
        var cameraDistance = Depth - perspectiveDepth;

        if (cameraDistance == 0) cameraDistance = 0.001f;

        _scale = MathF.Abs(perspectiveDepth / cameraDistance);

        // scale is focalLength divided by distance
        if (adjustScale)
        {
            var destination = Destination;
            destination.Width = (int)Math.Round(destination.Width * Math.Abs(_scale));
            destination.Height = (int)Math.Round(destination.Height * Math.Abs(_scale));
            Destination = destination;
        }

        _scale = 1 - _scale;
    }

    protected override void OnUpdate(GameTime gameTime, Controls[] controls)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            var actualPosition = Position;
            Position += (camera.View.Center.ToVector2() - Position) * _scale;
            renderer.Render(this, camera);
            Position = actualPosition;
        }
    }
}