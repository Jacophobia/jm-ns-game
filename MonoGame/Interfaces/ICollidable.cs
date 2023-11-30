using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Sprites;

namespace MonoGame.Interfaces;

public interface ICollidable
{
    public CollisionData CollisionData { get; }
    public Vector2 Position { get; }
    public Rectangle Bounds { get; }
    public Vector2 Velocity { get; set; }
    public float RestitutionCoefficient { get; }
    public bool IsStatic { get; }
    public float Mass { get; }

    public void Update(float deltaTime, IList<Controls> controls);

    public void HandleCollisionWith(ICollidable collidable, float deltaTime,
        Rectangle? overlap);

    public Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation);

    public bool CollidesWith(ICollidable rhs, out Rectangle? overlap);
}