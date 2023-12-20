using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Singletons;

namespace MonoGame.Output;

public class Writable : IWritable
{
    public SpriteFont Font { get; private set; }

    public string FontName
    {
        set => Font = FontManager.GetInstance()[value];
    }
    public string Text { get; set; }
    public Vector2 Position { get; set; }
    public Color TextColor { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public SpriteEffects Effects { get; set; }
    public float LayerDepth { get; set; }
}