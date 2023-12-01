using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public abstract class Player
{
    private readonly Camera _camera;

    protected Player(Camera camera)
    {
        _camera = camera;
    }

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(_camera.View))
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= _camera.View.X;
        relativeDestination.Y -= _camera.View.Y;

        OnDisplay(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null);
}