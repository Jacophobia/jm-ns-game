using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using OverWorld.Interactions;

namespace OverWorld.GameObjects;

public class Creature : IGameObject
{
    public Vector2 Position 
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
    public int Layer 
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
    public Vector2 Velocity 
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
    public Texture2D CurrentTexture => throw new System.NotImplementedException();
    public Rectangle Bounds => throw new System.NotImplementedException();
    public Vector2 PreviousVelocity => throw new System.NotImplementedException();
    public Vector2 PreviousPosition => throw new System.NotImplementedException();
    public float RestitutionCoefficient => throw new System.NotImplementedException();
    public float Mass => throw new System.NotImplementedException();
    public CollisionType CollisionType => throw new System.NotImplementedException();
    public bool IsStatic => throw new System.NotImplementedException();
    public void Update(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public void Render(IPlayer player)
    {
        throw new System.NotImplementedException();
    }
}