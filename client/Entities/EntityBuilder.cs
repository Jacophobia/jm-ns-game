using System;
using System.Collections.Generic;
using System.Linq;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.Entities;

public class EntityBuilder
{
    private bool TextureSet
    {
        set => _requiredFields[0] = value;
    }

    private bool DestinationSet
    {
        set => _requiredFields[1] = value;
    }
    private bool SourceSet
    {
        set => _requiredFields[2] = value;
    }
    private bool ColorSet
    {
        set => _requiredFields[3] = value;
    }
    private bool RotationSet
    {
        set => _requiredFields[4] = value;
    }
    private bool OriginSet
    {
        set => _requiredFields[5] = value;
    }
    private bool EffectSet
    {
        set => _requiredFields[6] = value;
    }
    private bool DepthSet
    {
        set => _requiredFields[7] = value;
    }
    private bool VelocitySet
    {
        set => _requiredFields[9] = value;
    }

    private readonly bool[] _requiredFields = new bool[10];
    private readonly ICollection<Action> _addDecorators;
    private Entity _entity;

    public EntityBuilder(Texture2D texture = null, Rectangle? destination = null, Rectangle? source = null,
        Color? color = null, float? rotation = null, Point? origin = null, SpriteEffects? effect = null,
        float? depth = null, Vector2? velocity = null)
    {
        _entity = new BaseEntity();

        if (texture != null)
        {
            _entity.Texture = texture;
            _entity.Sprite = new Sprite(texture);
            TextureSet = true;
        }

        if (destination.HasValue)
        {
            _entity.Destination = destination.Value;
            DestinationSet = true;
        }

        if (source.HasValue)
        {
            _entity.Source = source.Value;
            SourceSet = true;
        }

        if (color.HasValue)
        {
            _entity.Color = color.Value;
            ColorSet = true;
        }

        if (rotation.HasValue)
        {
            _entity.Rotation = rotation.Value;
            RotationSet = true;
        }

        if (origin.HasValue)
        {
            _entity.Origin = new Vector2(origin.Value.X, origin.Value.Y);
            OriginSet = true;
        }

        if (effect.HasValue)
        {
            _entity.Effect = effect.Value;
            EffectSet = true;
        }

        if (depth.HasValue)
        {
            _entity.Depth = depth.Value;
            DepthSet = true;
        }

        if (velocity.HasValue)
        {
            _entity.Velocity = velocity.Value;
            VelocitySet = true;
        }
    }
    
    public EntityBuilder SetTexture(Texture2D texture)
    {
        _entity.Texture = texture;
        _entity.Sprite = new Sprite(texture);
        TextureSet = true;
        return this;
    }

    public EntityBuilder SetDestination(Rectangle destination)
    {
        _entity.Destination = destination;
        DestinationSet = true;
        return this;
    }

    public EntityBuilder SetSource(Rectangle source)
    {
        _entity.Source = source;
        SourceSet = true;
        return this;
    }

    public EntityBuilder SetColor(Color color)
    {
        _entity.Color = color;
        ColorSet = true;
        return this;
    }

    public EntityBuilder SetRotation(float rotation)
    {
        _entity.Rotation = rotation;
        RotationSet = true;
        return this;
    }

    public EntityBuilder SetOrigin(Point origin)
    {
        _entity.Origin = new Vector2(origin.X, origin.Y);
        OriginSet = true;
        return this;
    }

    public EntityBuilder SetEffect(SpriteEffects effect)
    {
        _entity.Effect = effect;
        EffectSet = true;
        return this;
    }

    public EntityBuilder SetDepth(float depth)
    {
        _entity.Depth = depth;
        DepthSet = true;
        return this;
    }

    public EntityBuilder SetVelocity(Vector2 velocity)
    {
        _entity.Velocity = velocity;
        VelocitySet = true;
        return this;
    }

    public EntityBuilder AddDecorator<T>(params object[] parameters) where T : EntityDecorator
    {
        // because the constructor may need info from one of the 
        //  entities fields, we need to add them after all fields have 
        //  been set
        _addDecorators.Add(() => Entity.AddDecorator<T>(ref _entity, parameters));
        return this;
    }

    // Build method
    public Entity Build()
    {
        if (!_requiredFields.All(set => set))
            throw new InvalidOperationException("Cannot build BaseEntity: Not all fields have been set.");
        
        foreach (var addDecorator in _addDecorators)
        {
            addDecorator();
        }
        return _entity;

    }
}