using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Input;
using Shared.Output;

namespace Shared.Players;

public abstract class Player : IPlayer
{
    private readonly Camera _perspective;
    private readonly IControlSource _source;

    public Guid Id { get; }
    public Rectangle Perspective => _perspective.View;
    public float Depth => _perspective.Depth;
    public float FocalLength => Camera.FocalLength;
    public Controls Controls { get; private set; }

    protected Player(Guid id, Camera perspective, IControlSource source)
    {
        Id = id;
        _perspective = perspective;
        _source = source;
        Controls = Controls.None;
    }

    public void Update(float deltaTime)
    {
        Controls = _source.GetControls(this);
        
        _perspective.Update(deltaTime, Controls);
    }

    public void Follow(Func<Vector3> target)
    {
        _perspective.SetFocus(target);
    }

    public abstract void BeginDisplay();

    public void Display(Texture2D texture, Rectangle destination, float depth,
        Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect)
    {
        if (!(destination.Intersects(_perspective.View) || depth < _perspective.Depth))
            return;

        destination.X -= _perspective.View.X;
        destination.Y -= _perspective.View.Y;

        OnDisplay(texture, destination, source, color, rotation, origin, effect, depth);
    }
    
    protected abstract void OnDisplay(Texture2D texture, Rectangle destination,
        Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth);
    
    public abstract void EndDisplay();
}