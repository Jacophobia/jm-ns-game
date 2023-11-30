using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.Sprites;

namespace MonoGame.Entities;

public sealed class BaseEntity : Entity
{
    private int _depth;
    private Rectangle _destination;
    private CollisionData _collisionData;
    private Texture2D _texture;

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, int width, int height)
    {
        Texture = texture;
        Position = position;
        Velocity = velocity;
        Destination = new Rectangle((int)MathF.Round(position.X), (int)MathF.Round(position.Y), width, height);
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float scale) : this(texture, position,
        velocity, (int)MathF.Round(texture.Width * scale), (int)MathF.Round(texture.Height * scale))
    {
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, int width, int height,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects? effect = null, int? depth = null)
        : this(texture, position, velocity, width, height)
    {
        SetOptionalValues(source, color, rotation, origin, effect, depth);
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float scale,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects? effect = null, int? depth = null)
        : this(texture, position, velocity, scale)
    {
        SetOptionalValues(source, color, rotation, origin, effect, depth);
    }

    public override Texture2D Texture
    {
        get => _texture;
        set
        {
            _collisionData = new CollisionData(value);
            Source = value.Bounds;
            Origin = value.Bounds.Center.ToVector2();
        }
    }

    public override Rectangle Destination
    {
        get
        {
            _destination.X = (int)MathF.Round(Position.X);
            _destination.Y = (int)MathF.Round(Position.Y);
            return _destination;
        }
        set => _destination = value;
    }

    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public override CollisionData CollisionData => _collisionData;
    public override Rectangle Source { get; set; }
    public override Color Color { get; set; } = Color.White;
    public override float Rotation { get; set; }
    public override Vector2 Origin { get; set; }
    public override SpriteEffects Effect { get; set; } = SpriteEffects.None;

    public override int Depth
    {
        get => _depth;
        set
        {
            if (value is > Renderer.MaxDepth or < Renderer.MinDepth)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Depth must be between {Renderer.MinDepth} and {Renderer.MinDepth}");
            _depth = value;
        }
    }

    public override Vector2 Position { get; set; }
    public override Vector2 Velocity { get; set; }
    public override float RestitutionCoefficient { get; set; } = 1;
    public override bool IsStatic { get; set; } = false;

    private void SetOptionalValues(Rectangle? source = null, Color? color = null, float? rotation = null,
        Vector2? origin = null, SpriteEffects? effect = null, int? depth = null)
    {
        if (source.HasValue) Source = source.Value;

        if (color.HasValue) Color = color.Value;

        if (rotation.HasValue) Rotation = rotation.Value;

        if (origin.HasValue) Origin = origin.Value;

        if (effect.HasValue) Effect = effect.Value;

        if (depth.HasValue) Depth = depth.Value;
    }

    public override bool CollidesWith(ICollidable rhs, out Rectangle? overlap)
    {
        overlap = null;
        return false;
    }

    public override void Update(GameTime gameTime, IList<Controls> controls)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation)
    {
        return Vector2.Zero;
    }

    public override void HandleCollisionWith(ICollidable collidable, GameTime gameTime,
        Rectangle? overlap)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void Draw(Renderer renderer, Camera cameras)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }
}