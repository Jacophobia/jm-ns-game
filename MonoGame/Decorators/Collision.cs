﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

// ReSharper disable once ClassNeverInstantiated.Global
public class Collision : EntityDecorator
{
    public Collision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    private static bool AreMovingTowardsEachOther(Vector2 position1, Vector2 velocity1, Vector2 position2,
        Vector2 velocity2)
    {
        // Calculate position differences
        var deltaPosition = position2 - position1;

        // Calculate velocity differences
        var deltaVelocity = velocity2 - velocity1;

        // Calculate the dot product
        var dotProduct = Vector2.Dot(deltaPosition, deltaVelocity);

        // If the dot product is negative, objects are moving towards each other
        return dotProduct < 0;
    }

    protected override bool IsCollidingWith(ICollidable rhs, out Rectangle? overlap)
    {
        if (IsStatic && rhs.IsStatic)
        {
            overlap = null;
            return false;
        }
        
        // Find the intersection rectangle
        overlap = Rectangle.Intersect(Bounds, rhs.Bounds);

        // TODO: there is an issue where if something gets completely enveloped into something else between frames, it freezes the game
        // TODO: we also need to incorporate the Source rectangle in this so that collisions are calculated correctly when a sprite sheet is used
        return overlap is not { IsEmpty: true }
                && AreMovingTowardsEachOther(Destination.Center.ToVector2(), Velocity, overlap.Value.Center.ToVector2(), rhs.Velocity) 
                && CollisionData.Collides(Bounds, rhs.CollisionData, rhs.Bounds, overlap.Value);
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle? overlap)
    {
        Debug.Assert(!IsStatic || !rhs.IsStatic);
        Debug.Assert(rhs != null);
        Debug.Assert(overlap != null);

        var initialMagnitude = (Velocity + rhs.Velocity).Length();

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;
        
        var collisionCoordinate = overlap.Value.Center.ToVector2();

        // Calculate the normal (n) and tangential (t) direction vectors
        var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);
        
        // Decompose velocities into normal and tangential components
        var v1N = lhsVelocity.X * lhsNormal.X + lhsVelocity.Y * lhsNormal.Y; // Dot product
        var v1T = -lhsVelocity.X * lhsNormal.Y + lhsVelocity.Y * lhsNormal.X; // Perpendicular dot product

        var lhsRestitution = Math.Max(overlap.Value.Mass() / Mass, RestitutionCoefficient);
        var rhsRestitution = Math.Max(overlap.Value.Mass() / rhs.Mass, rhs.RestitutionCoefficient);

        if (IsStatic)
        {
            // If this object is static, only adjust the other object's velocity
            var newV2N = -rhsRestitution *
                         (rhsVelocity.X * rhsNormal.X + rhsVelocity.Y * rhsNormal.Y);
            rhsVelocity.X = newV2N * rhsNormal.X - (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.Y;
            rhsVelocity.Y = newV2N * rhsNormal.Y + (-rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X) * rhsNormal.X;
        }
        else if (rhs.IsStatic)
        {
            // Collision with a static object
            var newV1N = -lhsRestitution * v1N;

            // Recompose velocity for the dynamic object
            lhsVelocity.X = newV1N * lhsNormal.X - v1T * lhsNormal.Y;
            lhsVelocity.Y = newV1N * lhsNormal.Y + v1T * lhsNormal.X;
        }
        else
        {
            // Collision with another dynamic object
            var v2N = rhsVelocity.X * rhsNormal.X + rhsVelocity.Y * rhsNormal.Y;
            var v2T = -rhsVelocity.X * rhsNormal.Y + rhsVelocity.Y * rhsNormal.X;

            // Apply the restitution coefficient
            var combinedRestitution = (lhsRestitution + rhsRestitution) / 2f;

            // Exchange normal components in an inelastic collision
            var newV1N = combinedRestitution * (v1N * (Mass - rhs.Mass) + 2f * rhs.Mass * v2N) /
                         (Mass + rhs.Mass);
            var newV2N = combinedRestitution * (v2N * (rhs.Mass - Mass) + 2f * Mass * v1N) /
                         (Mass + rhs.Mass);

            // Recompose velocities for both objects
            lhsVelocity.X = newV1N * lhsNormal.X - v1T * lhsNormal.Y;
            lhsVelocity.Y = newV1N * lhsNormal.Y + v1T * lhsNormal.X;
            rhsVelocity.X = newV2N * rhsNormal.X - v2T * rhsNormal.Y;
            rhsVelocity.Y = newV2N * rhsNormal.Y + v2T * rhsNormal.X;
        }

        var currentMagnitude = (lhsVelocity + rhsVelocity).Length();
        
        if (initialMagnitude != 0 && currentMagnitude >= initialMagnitude)
        {
            lhsVelocity *= initialMagnitude / currentMagnitude;
            rhsVelocity *= initialMagnitude / currentMagnitude;
        }

        Velocity = lhsVelocity;
        rhs.Velocity = rhsVelocity;
    }
}