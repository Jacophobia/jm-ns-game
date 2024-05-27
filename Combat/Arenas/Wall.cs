using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.Rendering;
using Shared.Updates;

namespace Combat.Arenas;

public class Wall : ICollidable, IUpdatable
{
    private Vector2 _velocity;
    private Vector2 _position;

    public Vector2 Position
    {
        get => _position;
        set
        {
            PreviousPosition = _position;
            _position = value;
        }
    }
    public Vector2 PreviousPosition { get; private set; }
    public Vector2 Velocity
    {
        get => _velocity;
        set
        {
            PreviousVelocity = _velocity;
            _velocity = value;
        }
    }
    public Vector2 PreviousVelocity { get; private set; }
    public int Layer 
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
    public Texture2D CurrentTexture => throw new System.NotImplementedException();
    public Rectangle Bounds => throw new System.NotImplementedException();
    public float RestitutionCoefficient => throw new System.NotImplementedException();
    public float Mass => throw new System.NotImplementedException();
    public CollisionType CollisionType => throw new System.NotImplementedException();
    public bool IsStatic => throw new System.NotImplementedException();
    public Wall()
    {
        throw new System.NotImplementedException();
    }

    public void Render(params IRenderer[] renderer)
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }
}