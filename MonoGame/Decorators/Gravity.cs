using Microsoft.Xna.Framework;
using MonoGame.Constants;
using MonoGame.Entities;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Gravity : EntityDecorator
{
    private static readonly Vector2 GravitationalAcceleration = new Vector2(0, 1) 
        * Physics.PixelsToMeterRatio 
        * Physics.GravitationalAcceleration;
    
    public Gravity(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(float deltaTime)
    {
        Velocity += GravitationalAcceleration * deltaTime;
    }
}