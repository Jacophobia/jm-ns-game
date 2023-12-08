using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public abstract class Player : IPlayer
{
    private readonly Camera _perspective;
    private readonly IControlSource _source;

    public Rectangle Perspective => _perspective.View;
    public float Depth => _perspective.Depth;
    public float FocalLength => Camera.FocalLength;
    public Controls Controls { get; private set; }

    protected Player(Camera perspective, IControlSource source)
    {
        _perspective = perspective;
        _source = source;
        Controls = Controls.None;
    }

    public void Update(float deltaTime)
    {
        Controls = _source.GetControls();
        
        _perspective.Update(deltaTime, Controls);
    }

    public abstract void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(_perspective.View) || renderable.Depth < _perspective.Depth)
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= _perspective.View.X;
        relativeDestination.Y -= _perspective.View.Y;

        OnDisplay(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null);
    
    public abstract void EndDisplay();
}