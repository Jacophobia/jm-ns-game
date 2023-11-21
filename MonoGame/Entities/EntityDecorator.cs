using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.Sprites;

namespace MonoGame.Entities;

public abstract class EntityDecorator : Entity
{
    private readonly Entity _base;

    protected internal EntityDecorator(Entity @base)
    {
        _base = @base;
    }

    public sealed override Sprite Sprite => _base.Sprite;

    public sealed override Texture2D Texture
    {
        get => _base.Texture;
        set => _base.Texture = value;
    }

    public sealed override Rectangle Destination
    {
        get => _base.Destination;
        set => _base.Destination = value;
    }

    public sealed override Rectangle Source
    {
        get => _base.Source;
        set => _base.Source = value;
    }

    public sealed override Color Color
    {
        get => _base.Color;
        set => _base.Color = value;
    }

    public sealed override float Rotation
    {
        get => _base.Rotation;
        set => _base.Rotation = value;
    }

    public sealed override Vector2 Origin
    {
        get => _base.Origin;
        set => _base.Origin = value;
    }

    public sealed override SpriteEffects Effect
    {
        get => _base.Effect;
        set => _base.Effect = value;
    }

    public sealed override int Depth
    {
        get => _base.Depth;
        set => _base.Depth = value;
    }

    public override Vector2 Position
    {
        get => _base.Position;
        set => _base.Position = value;
    }

    public sealed override Vector2 Velocity
    {
        get => _base.Velocity;
        set => _base.Velocity = value;
    }

    public sealed override float RestitutionCoefficient
    {
        get => _base.RestitutionCoefficient;
        set => _base.RestitutionCoefficient = value;
    }

    public sealed override bool IsStatic
    {
        get => _base.IsStatic;
        set => _base.IsStatic = value;
    }

    public sealed override void Update(GameTime gameTime, Controls controls)
    {
        OnUpdate(gameTime, controls);
        _base.Update(gameTime, controls);
    }

    protected abstract void OnUpdate(GameTime gameTime, Controls controls);

    public sealed override void HandleCollisionWith(ICollidable collidable, GameTime gameTime,
        Vector2? collisionLocation, Rectangle? overlap)
    {
        OnHandleCollisionWith(collidable, gameTime, collisionLocation, overlap);
        _base.HandleCollisionWith(collidable, gameTime, collisionLocation, overlap);
    }

    protected abstract void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public sealed override void HandleCollisionFrom(ICollidable collidable, GameTime gameTime,
        Vector2? collisionLocation, Rectangle? overlap)
    {
        OnHandleCollisionFrom(collidable, gameTime, collisionLocation, overlap);
        _base.HandleCollisionFrom(collidable, gameTime, collisionLocation, overlap);
    }

    protected abstract void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap);

    public sealed override void Draw(Renderer renderer, Camera camera)
    {
        OnDraw(renderer, camera);
        _base.Draw(renderer, camera);
    }

    protected abstract void OnDraw(Renderer renderer, Camera camera);
}