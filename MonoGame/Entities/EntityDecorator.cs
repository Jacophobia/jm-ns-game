using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Collision;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Entities;

public abstract class EntityDecorator : Entity
{
    private readonly Entity _base;

    protected EntityDecorator(Entity @base)
    {
        _base = @base;
    }

    public sealed override CollisionData CollisionData => _base.CollisionData;
    public sealed override Rectangle PreviousBounds => _base.PreviousBounds;

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

    public sealed override int Layer
    {
        get => _base.Layer;
        set => _base.Layer = value;
    }

    public sealed override float Mass
    {
        get => _base.Mass;
        set => _base.Mass = value;
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

    public override Vector2 PreviousVelocity
    {
        get => _base.PreviousVelocity;
        set => _base.PreviousVelocity = value;
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

    // protected abstract void Initialize();

    public sealed override void Update(float deltaTime)
    {
        BeforeUpdate(deltaTime);
        OnUpdate(deltaTime);
        AfterUpdate(deltaTime);
        _base.Update(deltaTime);
    }

    protected virtual void BeforeUpdate(float deltaTime) {}
    protected virtual void OnUpdate(float deltaTime) {}
    protected virtual void AfterUpdate(float deltaTime) {}

    public override bool CollidesWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        return _base.CollidesWith(rhs, deltaTime, out overlap) || IsCollidingWith(rhs, deltaTime, out overlap);
    }

    protected virtual bool IsCollidingWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        overlap = null;
        return false;
    }

    public sealed override void HandleCollisionWith(ICollidable collidable, float deltaTime, Rectangle overlap)
    {
        BeforeHandleCollisionWith(collidable, deltaTime, overlap);
        OnHandleCollisionWith(collidable, deltaTime, overlap);
        AfterHandleCollisionWith(collidable, deltaTime, overlap);
        _base.HandleCollisionWith(collidable, deltaTime, overlap);
    }


    protected virtual void BeforeHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap) {}
    protected virtual void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle overlap) {}
    protected virtual void AfterHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap) {}

    public sealed override Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation)
    {
        return OnCalculateCollisionNormal(collidable, collisionLocation) + _base.CalculateCollisionNormal(collidable, collisionLocation);
    }

    protected virtual Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        return Vector2.Zero;
    }

    public sealed override void Draw(IPlayer player)
    {
        BeforeDraw(player);
        OnDraw(player);
        AfterDraw(player);
        _base.Draw(player);
    }

    protected virtual void BeforeDraw(IPlayer player) {}
    protected virtual void OnDraw(IPlayer player) {}
    protected virtual void AfterDraw(IPlayer player) {}
}