using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.Players;

namespace OverWorld.GameObjects;

public abstract class GameObject : ICollidable
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
    public Vector2 Velocity
    {
        get => _velocity;
        set
        {
            PreviousVelocity = _velocity;
            _velocity = value;
        }
    }
    public abstract int Layer { get; set; }
    
    // get-only
    public Vector2 PreviousVelocity { get; private set; }
    public Vector2 PreviousPosition { get; private set; }
    public abstract Texture2D CurrentTexture { get; }
    public abstract Rectangle Bounds { get; }
    public abstract float RestitutionCoefficient { get; }
    public abstract float Mass { get; }
    public abstract CollisionType CollisionType { get; }
    public abstract bool IsStatic { get; }
    
    // methods
    public abstract void Update(float deltaTime);
    public abstract void Render(IPlayer player);
}