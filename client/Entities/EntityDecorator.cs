using IO.Input;
using IO.Output;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpatialPartition.Collision;

namespace client.Entities;

public abstract class EntityDecorator : Entity
{
    public sealed override Sprite Sprite
    {
        get => _base.Sprite; 
        set => _base.Sprite = value;
    }

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

    public sealed override float Depth
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

    private readonly Entity _base;
    
    protected EntityDecorator(Entity @base)
    {
        _base = @base;
    }

    public sealed override void Update(GameTime gameTime, Controls controls)
    {
        OnUpdate(gameTime, controls);
        _base.Update(gameTime, controls);
    }
    
    protected abstract void OnUpdate(GameTime gameTime, Controls controls);

    public sealed override void HandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        OnHandleCollisionWith(collidable, gameTime, collisionLocation, overlap);
        _base.HandleCollisionWith(collidable, gameTime, collisionLocation, overlap);
    }

    protected abstract void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap);
    
    public sealed override void HandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        OnHandleCollisionFrom(collidable, gameTime, collisionLocation, overlap);
        _base.HandleCollisionFrom(collidable, gameTime, collisionLocation, overlap);
    }

    protected abstract void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap);

    public sealed override void Draw(Camera camera)
    {
        OnDraw(camera);
        _base.Draw(camera);
    }

    protected abstract void OnDraw(Camera camera);
}