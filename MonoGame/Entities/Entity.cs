using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.Sprites;

namespace MonoGame.Entities;

public abstract class Entity : ICollidable
{
    Texture2D IRenderable.Texture => Texture;
    public abstract Texture2D Texture { get; set; }
    public abstract Rectangle Destination { get; set; }
    public abstract Rectangle Source { get; set; }
    public abstract Color Color { get; set; }
    public abstract float Rotation { get; set; }
    public abstract Vector2 Origin { get; set; }
    public abstract SpriteEffects Effect { get; set; }
    public abstract int Depth { get; set; }
    public abstract Sprite Sprite { get; }
    public abstract Vector2 Position { get; set; }
    public abstract Vector2 Velocity { get; set; }
    public abstract float RestitutionCoefficient { get; set; }
    public abstract bool IsStatic { get; set; }
    public float Mass => Destination.Width * Destination.Height;

    public abstract void HandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public abstract void HandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public abstract void Update(GameTime gameTime, Controls[] controls);
    public abstract void Draw(Renderer renderer, Camera camera);

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