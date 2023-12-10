using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Collision;
using MonoGame.Interfaces;

namespace MonoGame.Entities;

public abstract class Entity : ICollidable, IRenderable, IUpdatable
{
    public abstract Texture2D Texture { get; set; }
    public abstract Rectangle Destination { get; set; }
    public abstract Rectangle Source { get; set; }
    public abstract Color Color { get; set; }
    public abstract float Rotation { get; set; }
    public abstract Vector2 Origin { get; set; }
    public abstract SpriteEffects Effect { get; set; }
    public abstract int Layer { get; set; }
    public abstract CollisionData CollisionData { get; }
    public abstract Vector2 Position { get; set; }
    public abstract Rectangle PreviousBounds { get; }
    public abstract Vector2 Velocity { get; set; }
    public abstract Vector2 PreviousVelocity { get; set; }
    public abstract float RestitutionCoefficient { get; set; }
    public abstract bool IsStatic { get; set; }
    public abstract float Mass { get; set; }
    public Rectangle Bounds => Destination;
    public float Depth => Layer;

    public abstract void HandleCollisionWith(ICollidable collidable, float deltaTime,
        Rectangle overlap);

    public abstract Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation);
    public abstract bool CollidesWith(ICollidable rhs, float deltaTime, out Rectangle? overlap);

    public abstract void Update(float deltaTime);
    public abstract void Draw(IPlayer cameras);

    private T AddDecorator<T>(params object[] parameters) where T : EntityDecorator
    {
        var @params = new object[parameters.Length + 1];
        @params[0] = this;
        parameters.CopyTo(@params, 1);

        return (T)Activator.CreateInstance(typeof(T), @params);
    }

    internal static void AddDecorator<T>(ref Entity entity, params object[] parameters) where T : EntityDecorator
    {
        entity = entity.AddDecorator<T>(parameters);
    }
}