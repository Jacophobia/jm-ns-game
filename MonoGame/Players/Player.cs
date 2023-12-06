using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public abstract class Player : IPlayer
{
    private readonly Camera _perspective;

    public Rectangle Perspective => _perspective.View;
    public float Depth => _perspective.Position.Z;

    protected Player(Camera perspective)
    {
        _perspective = perspective;
    }

    public void Update(float deltaTime, Controls controls)
    {
        _perspective.Update(deltaTime, controls);
    }

    public abstract void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(_perspective.View))
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= _perspective.View.X;
        relativeDestination.Y -= _perspective.View.Y;

        OnDisplay(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null);
    
    public abstract void EndDisplay();
}