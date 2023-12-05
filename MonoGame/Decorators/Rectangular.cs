using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Rectangular : EntityDecorator
{
    public Rectangular(Entity @base) : base(@base)
    {
        // Initialization code if necessary
    }

    protected override Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        var lhsBounds = Bounds;
        var rhsBounds = rhs.Bounds;

        // Determine the side of the collision
        return CalculateCollisionNormal(lhsBounds, rhsBounds, collisionLocation);
    }

    private static Vector2 CalculateCollisionNormal(Rectangle lhsBounds, Rectangle rhsBounds, Vector2 collisionLocation)
    {
        // Center points of the rectangles
        var lhsCenter = lhsBounds.Center.ToVector2();
        var rhsCenter = rhsBounds.Center.ToVector2();

        // Distance between the centers
        var distance = rhsCenter - lhsCenter;

        // Half extents along each axis
        var lhsHalfWidth = lhsBounds.Width / 2f;
        var lhsHalfHeight = lhsBounds.Height / 2f;
        var rhsHalfWidth = rhsBounds.Width / 2f;
        var rhsHalfHeight = rhsBounds.Height / 2f;

        // Calculate overlap on each axis
        var overlapX = lhsHalfWidth + rhsHalfWidth - Math.Abs(distance.X);
        var overlapY = lhsHalfHeight + rhsHalfHeight - Math.Abs(distance.Y);

        // The axis with the smallest overlap determines the normal
        return overlapX < overlapY ? new Vector2(Math.Sign(distance.X), 0) : new Vector2(0, Math.Sign(distance.Y));
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, float deltaTime, Rectangle overlap)
    {
        const float percent = 1f; // Penetration percentage to correct
        const float slop = 0.01f; // Allowable penetration
        
        var collisionCoordinate = overlap.Center.ToVector2();

        var lhsNormal = CalculateCollisionNormal(rhs, collisionCoordinate);
        var rhsNormal = -rhs.CalculateCollisionNormal(this, collisionCoordinate);
        
        var lhsCorrection = Math.Max(overlap.Height - slop, 0.0f) / (1 / Mass + 1 / rhs.Mass) * percent * lhsNormal;
        var rhsCorrection = Math.Max(overlap.Height - slop, 0.0f) / (1 / rhs.Mass + 1 / Mass) * percent * rhsNormal;
        
        if (!IsStatic)
        {
            Position -= lhsCorrection / Mass;
        }
        if (!rhs.IsStatic)
        {
            rhs.Position += rhsCorrection / rhs.Mass;
        }
    }
}
