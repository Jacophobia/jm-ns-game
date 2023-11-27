using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Output;

namespace MonoGame.Interfaces;

public interface IRenderable
{
    internal Texture2D Texture { get; }
    public Rectangle Destination { get; set; }
    public Rectangle Source { get; }
    public Color Color { get; }
    public float Rotation { get; }
    public Vector2 Origin { get; }
    public SpriteEffects Effect { get; }
    public int Depth { get; }
    public void Draw(Renderer renderer, Camera[] cameras);
}