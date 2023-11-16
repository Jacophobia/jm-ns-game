﻿using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using Microsoft.Xna.Framework;
using SpatialPartition.Interfaces;

namespace client.Decorators;

public class Gravity : EntityDecorator
{
    public Gravity(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        // no new behavior to add
    }

    protected override void OnDraw(Renderer renderer, Camera camera)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        Velocity = new Vector2(Velocity.X, Velocity.Y - 9.8f * gameTime.DeltaTime());
    }
}