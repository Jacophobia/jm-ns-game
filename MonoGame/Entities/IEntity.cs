using Microsoft.Xna.Framework;
using MonoGame.Interfaces;

namespace MonoGame.Entities;

public interface IEntity
{
    // public Texture2D Texture { get; }
    // public Rectangle Destination { get; }
    // public Rectangle Source { get; }
    // public Color Color { get; }
    // public float Rotation { get; }
    // public Vector2 Origin { get; }
    // public SpriteEffects Effect { get; }
    // public float Depth { get; }
    
    // public CollisionData CollisionData { get; }
    // public Vector2 Position { get; set; }
    // public Rectangle PreviousBounds { get; }
    // public Vector2 Velocity { get; set; }
    // public Vector2 PreviousVelocity { get; }
    // public float RestitutionCoefficient { get; }
    // public float Mass { get; }
    
    public bool IsStatic { get; }
    public Rectangle Bounds { get; }
    public int Layer { get; }
    public void Update(float deltaTime);
    public void Render(IPlayer player);
    
    public void HandleCollisionWith(IEntity collidable, float deltaTime,
        Rectangle overlap);
    public Vector2 CalculateCollisionNormal(IEntity collidable, Vector2 collisionLocation);
    public bool CollidesWith(IEntity rhs, float deltaTime, out Rectangle? overlap);
}