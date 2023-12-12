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

    public PlayerId Id { get; }
    public Rectangle Perspective => _perspective.View;
    public float Depth => _perspective.Depth;
    public float FocalLength => Camera.FocalLength;
    public Controls Controls { get; private set; }

    protected Player(object id, Camera perspective, IControlSource source)
    {
        Id = new PlayerId(id);
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
        if (!(destination ?? renderable.Destination).Intersects(_perspective.View) || renderable.Depth < _perspective.Depth)
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= _perspective.View.X;
        relativeDestination.Y -= _perspective.View.Y;

        OnDisplay(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

    public void Display(SpriteFont font, string text, Vector2 position, Color color)
    {
        OnDisplay(font, text, position, color);
    }

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null);

    protected abstract void OnDisplay(SpriteFont font, string text, Vector2 position, Color color);
    
    public abstract void EndDisplay();
}