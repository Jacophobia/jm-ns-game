using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.Controllables;
using Shared.Extensions;
using Shared.Rendering;
using Shared.View;

namespace Combat.Fighters;

public class Fighter : Controllable, ICollidable, IRenderable, IViewable
{
    private const int CombatLayer = 0;
    private readonly uint _totalHealth;
    private uint _currentHealth;
    private Vector2 _velocity;
    private Vector2 _position;

    public uint Health => _currentHealth;

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
    public Texture2D CurrentTexture => throw new System.NotImplementedException();
    public Rectangle Bounds => throw new System.NotImplementedException();
    public float RestitutionCoefficient => throw new System.NotImplementedException();
    
    public float Mass => Bounds.Mass();
    public CollisionType CollisionType => CollisionType.Rectangular;
    public bool IsStatic => false;
    public int Layer => CombatLayer;
    public Vector3 CurrentPosition => new()
    {
        X = _position.X,
        Y = _position.Y,
        Z = Layer
    };
    
    public Fighter(IController controller, uint totalHealth, uint currentHealth)
        : base(controller)
    {
        _totalHealth = totalHealth;
        _currentHealth = currentHealth;
        throw new System.NotImplementedException();
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        throw new System.NotImplementedException();
    }

    public void Render(IRenderer renderer, Camera camera)
    {
        renderer.Render(
            camera, 
            CurrentTexture, 
            Bounds, 
            CurrentTexture.Bounds, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            SpriteEffects.None, 
            Layer
        );
    }

    public void Damage(uint amount)
    {
        if (amount > _currentHealth)
        {
            _currentHealth = 0;
        }
        else
        {
            _currentHealth -= amount;
        }
    }
}