using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Collision;
using Shared.Players;

namespace OverWorld.GameObjects;

public class Creature : GameObject
{
    public override int Layer 
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
    public override Texture2D CurrentTexture => throw new System.NotImplementedException();
    public override Rectangle Bounds => throw new System.NotImplementedException();
    public override float RestitutionCoefficient => throw new System.NotImplementedException();
    public override float Mass => throw new System.NotImplementedException();
    public override CollisionType CollisionType => throw new System.NotImplementedException();
    public override bool IsStatic => throw new System.NotImplementedException();
    
    public override void Update(float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public override void Render(IPlayer player)
    {
        throw new System.NotImplementedException();
    }
}