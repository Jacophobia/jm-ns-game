using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Rendering;

public struct Renderable
{
    public string TextureName { get; set; }
    public Rectangle Destination { get; set; }
    public Rectangle Source { get; set; }
    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects Effect { get; set; }
    public float Depth { get; set; }
}