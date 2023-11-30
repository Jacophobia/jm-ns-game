﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Sprites;

namespace MonoGame.Interfaces;

public interface ICollidable
{
    public CollisionData CollisionData { get; }
    public Vector2 Position { get; set; }
    public Rectangle Bounds { get; }
    public Vector2 Velocity { get; set; }
    public float RestitutionCoefficient { get; }
    public bool IsStatic { get; }
    public float Mass { get; }

    public void Update(GameTime gameTime, IList<Controls> controls);

    public void HandleCollisionWith(ICollidable collidable, GameTime gameTime,
        Rectangle? overlap);

    public Vector2 CalculateCollisionNormal(ICollidable collidable, Vector2 collisionLocation);

    public bool CollidesWith(ICollidable rhs, out Rectangle? overlap);
}