using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using OverWorld.Interactions;

namespace OverWorld.GameObjects;

public interface IGameObject
{
    // modifiable
    public Vector2 Position { get; set; }
    public int Layer { get; set; }
    public Vector2 Velocity { get; set; }
    
    // unmodifiable
    public Texture2D CurrentTexture { get; }
    public Rectangle Bounds { get; }
    public Vector2 PreviousVelocity { get; }
    public Vector2 PreviousPosition { get; }
    public float RestitutionCoefficient { get; }
    public float Mass { get; }
    public CollisionType CollisionType { get; }
    public bool IsStatic { get; }
    
    // methods
    public void Update(float deltaTime);
    public void Render(IPlayer player);
}