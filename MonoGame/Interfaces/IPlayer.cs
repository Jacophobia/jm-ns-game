using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Players;

namespace MonoGame.Interfaces;

public interface IPlayer
{
    public PlayerId Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; }

    public void Follow(IRenderable target);

    public void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null);

    public void Display(IWritable writable);
    
    public void Update(float deltaTime);

    public void EndDisplay();
}