using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Basic : IPlayer
{
    private Renderer _renderer;
    
    public Basic(Rectangle perspective, float depth, Renderer renderer)
    {
        Perspective = perspective;
        Depth = depth;
        _renderer = renderer;
    }

    public Rectangle Perspective { get; }
    public float Depth { get; }
    public void BeginDisplay()
    { 
        _renderer.Begin();
    }

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null, Rectangle? source = null,
        Color? color = null, float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None,
        int? depth = null)
    {
        _renderer.Draw(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Update(float deltaTime, Controls controls)
    {
        // :)
    }

    public void EndDisplay()
    {
        _renderer.End();
    }
}