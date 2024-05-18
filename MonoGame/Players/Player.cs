using System;
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

    public void Follow(IRenderable target)
    {
        _perspective.SetFocus(target);
    }

    public abstract void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(_perspective.View) || (depth ?? renderable.Depth) < _perspective.Depth)
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= _perspective.View.X;
        relativeDestination.Y -= _perspective.View.Y;

        OnDisplay(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

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

    public void Display(IWritable writable, SpriteFont font = null, 
    string text = null, Vector2? position = null, Color? color = null, 
    float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, 
    float? depth = null)
    {
        OnDisplay(writable, font, text, position, color, rotation, origin, scale, effect, depth);
    }

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null);
    
    protected abstract void OnDisplay(Texture2D texture, Rectangle destination,
        Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect, float depth);

    protected abstract void OnDisplay(IWritable writable, SpriteFont font = null, 
        string text = null, Vector2? position = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null);
    
    public abstract void EndDisplay();
}