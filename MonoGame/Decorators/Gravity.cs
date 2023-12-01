using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Input;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Gravity : EntityDecorator
{
    private const float GravitationalAcceleration = 9.8f;
    private static readonly Vector2 GravitationalDirection = new(0, 30);
    
    public Gravity(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(float deltaTime, Controls controls)
    {
        Velocity += GravitationalDirection * GravitationalAcceleration * deltaTime;
    }
}