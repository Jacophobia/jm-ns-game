using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.Players;
using Shared.Singletons;

namespace Combat.Arenas;

public class Floor : ICollidable
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
    public Texture2D CurrentTexture { get; }

    public Rectangle Bounds => throw new System.NotImplementedException();
    public float RestitutionCoefficient => throw new System.NotImplementedException();
    public float Mass => throw new System.NotImplementedException();
    public CollisionType CollisionType => throw new System.NotImplementedException();
    public bool IsStatic => throw new System.NotImplementedException();
    public Floor()
    {
        CurrentTexture = TextureManager.GetInstance()["Arena/Floor/Test"];
        _velocity = Vector2.Zero;
        PreviousVelocity = Vector2.Zero;
        throw new System.NotImplementedException();
    }
    
    public void Update(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public void Render(IPlayer player)
    {
        throw new System.NotImplementedException();
    }
}