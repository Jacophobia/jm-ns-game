using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.Entities;

public abstract class EntityDecorator : Entity
{
    private readonly Entity _base;

    protected EntityDecorator(Entity @base)
    {
        _base = @base;
    }
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

    public override bool CollidesWith(Entity rhs, float deltaTime)
    {
        return _base.CollidesWith(rhs, deltaTime) || IsCollidingWith(rhs, deltaTime);
    }

    protected virtual bool IsCollidingWith(Entity rhs, float deltaTime)
    {
        return false;
    }

    public sealed override void HandleCollisionWith(Entity collidable, float deltaTime)
    {
        BeforeHandleCollisionWith(collidable, deltaTime);
        OnHandleCollisionWith(collidable, deltaTime);
        AfterHandleCollisionWith(collidable, deltaTime);
        _base.HandleCollisionWith(collidable, deltaTime);
    }


    protected virtual void BeforeHandleCollisionWith(Entity rhs, float deltaTime) {}
    protected virtual void OnHandleCollisionWith(Entity rhs, float deltaTime) {}
    protected virtual void AfterHandleCollisionWith(Entity rhs, float deltaTime) {}

    public sealed override Vector2 CalculateCollisionNormal(Entity collidable, Vector2 collisionLocation)
    {
        return OnCalculateCollisionNormal(collidable, collisionLocation) + _base.CalculateCollisionNormal(collidable, collisionLocation);
    }

    protected virtual Vector2 OnCalculateCollisionNormal(Entity rhs, Vector2 collisionLocation)
    {
        return Vector2.Zero;
    }

    public sealed override void Render(IPlayer player)
    {
        BeforeDraw(player);
        OnDraw(player);
        AfterDraw(player);
        _base.Render(player);
    }

    protected virtual void BeforeDraw(IPlayer player) {}
    protected virtual void OnDraw(IPlayer player) {}
    protected virtual void AfterDraw(IPlayer player) {}
}