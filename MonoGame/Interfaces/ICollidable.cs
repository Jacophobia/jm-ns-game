using Microsoft.Xna.Framework;
using MonoGame.Collision;
using MonoGame.Input;

namespace MonoGame.Interfaces;

public interface ICollidable
{
    public CollisionData CollisionData { get; }
    public Vector2 Position { get; set; }
    public Rectangle Bounds { get; }
    public Vector2 Velocity { get; set; }
    public float RestitutionCoefficient { get; }
    public bool IsStatic { get; }
    public float Mass { get; }

    public void Update(float deltaTime, Controls controls);

    public void HandleCollisionWith(ICollidable collidable, float deltaTime,
        Rectangle? overlap);

    public Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation);

    public bool CollidesWith(ICollidable rhs, float deltaTime, out Rectangle? overlap);
}