using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public abstract class Component : IRenderable
{
    private bool _wasMouseButtonDown;
    public event Action SelectEvent;

    protected Component(Texture2D texture, Rectangle destination)
    {
        Texture = texture;
        Destination = destination;
        Source = texture.Bounds;
        Color = Color.White;
        Rotation = 0f;
        Origin = Vector2.Zero;
        Effect = SpriteEffects.None;
        Depth = 0f;
        
        _wasMouseButtonDown = false;
    }

    public Texture2D Texture { get; }
    public Rectangle Destination { get; }
    public Rectangle Source { get; }
    public Color Color { get; set; }
    public float Rotation { get; }
    public Vector2 Origin { get; }
    public SpriteEffects Effect { get; }
    public float Depth { get; }

    private bool WasClicked(Point mousePosition)
    {
        return Destination.Contains(mousePosition);
    }

    protected abstract void OnSelect();

    public void Update(bool isSelected, Point mousePosition, bool isMouseButtonDown)
    {
        if (isSelected || (isMouseButtonDown && !_wasMouseButtonDown && WasClicked(mousePosition)))
        {
            SelectEvent?.Invoke();
            OnSelect();
        }

        _wasMouseButtonDown = isMouseButtonDown;
    }
    
    public void Render(IPlayer player)
    {
        player.Display(this);
        OnRender(player);
    }

    protected abstract void OnRender(IPlayer player);
}