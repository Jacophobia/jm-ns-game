using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Collision;
using MonoGame.Extensions;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Entities;

public sealed class BaseEntity : Entity
{
    private int _depth;
    private Rectangle _destination;
    private CollisionData _collisionData;
    private Texture2D _texture;
    private float? _mass;
    private Vector2 _position;
    private Vector2 _previousPosition;
    private Vector2 _velocity;

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, int width, int height)
    {
        Texture = texture;
        Position = position;
        PreviousVelocity = velocity * 10 + Vector2.One;
        Velocity = velocity;
        _previousPosition = position * 10 + Vector2.One;
        Destination = new Rectangle((int)MathF.Round(position.X), (int)MathF.Round(position.Y), width, height);
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float scale) : this(texture, position,
        velocity, (int)MathF.Round(texture.Width * scale), (int)MathF.Round(texture.Height * scale))
    {
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, int width, int height,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects? effect = null, int? depth = null, float? mass = null)
        : this(texture, position, velocity, width, height)
    {
        SetOptionalValues(source, color, rotation, origin, effect, depth, mass);
    }

    internal BaseEntity(Texture2D texture, Vector2 position, Vector2 velocity, float scale,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects? effect = null, int? depth = null, float? mass = null)
        : this(texture, position, velocity, scale)
    {
        SetOptionalValues(source, color, rotation, origin, effect, depth, mass);
    }

    public override Texture2D Texture
    {
        get => _texture;
        set
        {
            _collisionData = new CollisionData(value);
            Source = value.Bounds;
            Origin = value.Bounds.Location.ToVector2();
            _texture = value;
        }
    }

    public override Rectangle Destination
    {
        get
        {
            _destination.Location = Vector2.Round(Position).ToPoint();
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

    public override int Layer
    {
        get => _depth;
        set
        {
            if (value is > Renderer.MaxDepth or < Renderer.MinDepth)
                Debug.WriteLine($"Depth must be between {Renderer.MinDepth} and {Renderer.MaxDepth}");
            _depth = value;
        }
    }

    public override Vector2 Position
    {
        get => _position;
        set
        {
            if (IsStatic)
                return;

            if (Vector2.Round(value) != Vector2.Round(_position) && Vector2.Round(_position) != Vector2.Round(_previousPosition))
                _previousPosition = _position;
            
            _position = value;

            Debug.Assert(_position != _previousPosition);
        }
    }

    public override Rectangle PreviousBounds
    {
        get
        {
            var bounds = Bounds;
            bounds.Location = Vector2.Round(_previousPosition).ToPoint();
            return bounds;
        }
    }

    public override Vector2 Velocity
    {
        get => _velocity;
        set
        {
            if (IsStatic)
                return;

            if ((value - _velocity).Length() > 0.01f && (_velocity - PreviousVelocity).Length() > 0.01f)
                PreviousVelocity = _velocity;
            
            _velocity = value;
            
            Debug.Assert(_velocity != PreviousVelocity);
        }
    }

    public override Vector2 PreviousVelocity { get; set; }

    public override float RestitutionCoefficient { get; set; } = 1;
    public override bool IsStatic { get; set; } = false;

    public override float Mass
    {
        get
        {
            if (IsStatic)
                return float.PositiveInfinity;
            
            return _mass ?? Destination.Mass();
        }
        set => _mass = value;
    }

    private void SetOptionalValues(Rectangle? source, Color? color, float? rotation,
        Vector2? origin, SpriteEffects? effect, int? depth, float? mass)
    {
        if (source.HasValue) Source = source.Value;

        if (color.HasValue) Color = color.Value;

        if (rotation.HasValue) Rotation = rotation.Value;

        if (origin.HasValue) Origin = origin.Value;

        if (effect.HasValue) Effect = effect.Value;

        if (depth.HasValue) Layer = depth.Value;

        if (mass.HasValue) Mass = mass.Value;
    }

    public override bool CollidesWith(ICollidable rhs, float deltaTime, out Rectangle? overlap)
    {
        overlap = null;
        return false;
    }

    public override void Update(float deltaTime)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation)
    {
        return Vector2.Zero;
    }

    public override void HandleCollisionWith(ICollidable collidable, float deltaTime,
        Rectangle overlap)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void Draw(IPlayer cameras)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }
}