using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.Entities;

public class EntityBuilder
{
    private readonly Stack<Action> _addDecorators;
    private Entity _entity;

    public EntityBuilder(Texture2D texture, Vector2 position, Vector2 velocity, int? width = null, int? height = null,
        float? scale = null, Rectangle? source = null, Color? color = null, float? rotation = null,
        Vector2? origin = null, SpriteEffects? effect = null, int? depth = null, bool isStatic = false)
    {
        if (width.HasValue && height.HasValue && !scale.HasValue)
        {
            _entity = new BaseEntity(texture, position, velocity, width.Value, height.Value, source, color, rotation,
                origin, effect, depth);
        }
        else if (!width.HasValue && !height.HasValue && scale.HasValue)
        {
            _entity = new BaseEntity(texture, position, velocity, scale.Value, source, color, rotation, origin, effect,
                depth);
        }
        else
        {
            Debug.Assert(false,
                "There should either be a single scale with no width and height or a width and height with no scale");
            throw new Exception("An EntityBuilder instance was created without valid parameters");
        }

        _entity.IsStatic = isStatic;

        _addDecorators = new Stack<Action>();
    }

    public EntityBuilder SetTexture(Texture2D texture)
    {
        _entity.Texture = texture;
        return this;
    }

    public EntityBuilder SetPosition(Vector2 position)
    {
        _entity.Position = position;
        return this;
    }

    public EntityBuilder SetSource(Rectangle source)
    {
        _entity.Source = source;
        return this;
    }

    public EntityBuilder SetColor(Color color)
    {
        _entity.Color = color;
        return this;
    }

    public EntityBuilder SetRotation(float rotation)
    {
        _entity.Rotation = rotation;
        return this;
    }

    public EntityBuilder SetOrigin(Vector2 origin)
    {
        _entity.Origin = origin;
        return this;
    }

    public EntityBuilder SetEffect(SpriteEffects effect)
    {
        _entity.Effect = effect;
        return this;
    }

    public EntityBuilder SetDepth(int depth)
    {
        _entity.Depth = depth;
        return this;
    }

    public EntityBuilder SetVelocity(Vector2 velocity)
    {
        _entity.Velocity = velocity;
        return this;
    }

    public EntityBuilder SetStatic(bool isStatic)
    {
        _entity.IsStatic = isStatic;
        return this;
    }

    public EntityBuilder AddDecorator<T>(params object[] parameters) where T : EntityDecorator
    {
        // because the constructor may need info from one of the 
        //  entities fields, we need to add them after all fields have 
        //  been set
        _addDecorators.Push(() => Entity.AddDecorator<T>(ref _entity, parameters));
        return this;
    }

    // Build method
    public Entity Build()
    {
        while (_addDecorators.TryPop(out var decorate))
        {
            decorate();
        }
        return _entity;
    }
}