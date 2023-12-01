using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public abstract class Player
{
    public Camera Perspective { get; }

    protected Player(Camera perspective)
    {
        Perspective = perspective;
    }

    public void Update(float deltaTime, IList<Controls> controls)
    {
        Perspective.Update(deltaTime, controls);
    }

    public abstract void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null)
    {
        if (!(destination ?? renderable.Destination).Intersects(Perspective.View))
            return;

        var relativeDestination = destination ?? renderable.Destination;

        relativeDestination.X -= Perspective.View.X;
        relativeDestination.Y -= Perspective.View.Y;

        OnDisplay(renderable, texture, relativeDestination, source, color, rotation, origin, effect, depth);
    }

    public abstract void EndDisplay();

    protected abstract void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, 
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null, 
        SpriteEffects effect = SpriteEffects.None, int? depth = null);
}