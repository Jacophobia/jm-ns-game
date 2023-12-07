using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public abstract class Player : IPlayer
{
    private readonly Camera _perspective;
    private readonly IControlListener _listener;
    private readonly List<IUpdatable> _updatables;

    public Rectangle Perspective => _perspective.View;
    public float Depth => _perspective.Depth;
    public float FocalLength => Camera.FocalLength;

    protected Player(Camera perspective, IControlListener listener)
    {
        _perspective = perspective;
        _listener = listener;
        _updatables = new List<IUpdatable>();
    }

    public void Update(float deltaTime)
    {
        var controls = _listener.GetControls();
        
        _perspective.Update(deltaTime, controls);
        
        foreach (var updatable in _updatables)
        {
            updatable.Update(deltaTime, controls);
        }
    }

    public void Add(IUpdatable updatable)
    {
        _updatables.Add(updatable);
    }

    public void Remove(IUpdatable updatable)
    {
        _updatables.Remove(updatable);
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