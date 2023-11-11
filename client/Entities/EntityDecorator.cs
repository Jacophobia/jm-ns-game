using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.Entities;

public abstract class EntityDecorator : Entity
{
    private readonly Entity _base;

    protected EntityDecorator(Entity @base)
    {
        _base = @base;
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

    public sealed override Vector2 Velocity
    {
        get => _base.Velocity;
        set => _base.Velocity = value;
    }

    public sealed override void Update(GameTime gameTime)
    {
        OnUpdate(gameTime);
    }

    protected abstract void OnUpdate(GameTime gameTime);
}