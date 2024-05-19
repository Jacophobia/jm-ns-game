using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared.Collision;

public interface ICollidable
{
    public Vector2 Position { get; set; }
    public int Layer { get; }
    public Vector2 Velocity { get; set; }
    
    public Texture2D CurrentTexture { get; }
    public CollisionType CollisionType { get; }
    public bool IsStatic { get; }
    public Rectangle Bounds { get; }
    public Vector2 PreviousPosition { get; }
    public float RestitutionCoefficient { get; }
    public float Mass { get; }
}