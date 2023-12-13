using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Interfaces;

public interface IWritable
{
    public SpriteFont Font { get; }
    public string Text { get; }
    public Vector2 Position { get; }
    public Color TextColor { get; }
    public float Rotation { get; }
    public Vector2 Origin { get; }
    public Vector2 Scale { get; }
    public SpriteEffects Effects { get; }
    public float LayerDepth { get; }
}