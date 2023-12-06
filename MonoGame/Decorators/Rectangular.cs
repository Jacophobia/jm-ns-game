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
        // nothing to initialize
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
        var normal = overlapX < overlapY ? new Vector2(Math.Sign(distance.X), 0) : new Vector2(0, Math.Sign(distance.Y));
        
        Debug.Assert(!float.IsNaN(normal.X) && !float.IsNaN(normal.Y));
        
        return normal;
    }
}