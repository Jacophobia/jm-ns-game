using System;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.Entities;

public abstract class Entity : IRenderable
{
    public abstract Texture2D Texture { get; set; }
    public abstract Rectangle Destination { get; set; }
    public abstract Rectangle Source { get; set; }
    public abstract Color Color { get; set; }
    public abstract float Rotation { get; set; }
    public abstract Vector2 Origin { get; set; }
    public abstract SpriteEffects Effect { get; set; }
    public abstract float Depth { get; set; }
    public abstract Vector2 Velocity { get; set; }
    public abstract void Update(GameTime gameTime);

    private T AddDecorator<T>(params object[] parameters) where T : EntityDecorator
    {
        var @params = new object[parameters.Length + 1];
        @params[0] = this;
        parameters.CopyTo(@params, 1);
        
        return (T)Activator.CreateInstance(typeof(T), @params);
    }

    public static void AddDecorator<T>(ref Entity entity, params object[] parameters) where T : EntityDecorator
    {
        entity = entity.AddDecorator<T>(parameters);
    }
}