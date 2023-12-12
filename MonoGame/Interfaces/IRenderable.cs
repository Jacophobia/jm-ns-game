using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Interfaces;

public interface IRenderable
{
    public Texture2D Texture { get; }
    public Rectangle Destination { get; }
    public Rectangle Source { get; }
    public Color Color { get; }
    public float Rotation { get; }
    public Vector2 Origin { get; }
    public SpriteEffects Effect { get; }
    public float Depth { get; }
    public void Render(IPlayer player);
}