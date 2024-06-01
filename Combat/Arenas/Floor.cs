using System.Collections;
using System.Collections.Generic;
using Combat.Fighters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.DataStructures;
using Shared.Extensions;
using Shared.Rendering;
using Shared.Singletons;
using Shared.View;

namespace Combat.Arenas;

public class Floor : IEnumerable<FloorPanel>
{
    private readonly Deque<FloorPanel> _panels;
    private readonly bool _infinite;

    public Floor(bool infinite)
    {
        _infinite = infinite;
        
        // set floor panel starting positions
    }
    
    public void Update(GameTime gameTime, Fighter fighter1, Fighter fighter2)
    {
        if (!_infinite)
        {
            return;
        }
        
        // if fighters get close enough to a ledge, add a panel there
        
        // if fighters are far enough from a panel, remove it (fighters
        // will not be able to move away from eachother indefinitely so
        // the center of their two positions should be sufficient.
    }

    public IEnumerator<FloorPanel> GetEnumerator()
    {
        return _panels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class FloorPanel : ICollidable, IRenderable
{
    private Vector2 _velocity;
    private Vector2 _position;
    
    public FloorPanel()
    {
        CurrentTexture = TextureManager.GetInstance()["Arena/Floor/Test"];
        _velocity = Vector2.Zero;
        PreviousVelocity = Vector2.Zero;
        throw new System.NotImplementedException();
    }

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
    public Texture2D CurrentTexture { get; }

    public int Layer => 0;
    public Rectangle Bounds => new (0, 0, 2048, 128);
    // basically the amount of friction it has / its bounciness
    public float RestitutionCoefficient => 0.01f; 
    public float Mass => Bounds.Mass();
    public CollisionType CollisionType => CollisionType.Rectangular;
    public bool IsStatic => true;

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
}