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
        Rectangle lhsBounds = Destination;
        Rectangle rhsBounds = rhs.Destination;

        // Determine the side of the collision
        return CalculateCollisionNormal(lhsBounds, rhsBounds, collisionLocation);
    }

    private Vector2 CalculateCollisionNormal(Rectangle lhsBounds, Rectangle rhsBounds, Vector2 collisionLocation)
    {
        // Center points of the rectangles
        Vector2 lhsCenter = lhsBounds.Center.ToVector2();
        Vector2 rhsCenter = rhsBounds.Center.ToVector2();

        // Distance between the centers
        Vector2 distance = rhsCenter - lhsCenter;

        // Half extents along each axis
        float lhsHalfWidth = lhsBounds.Width / 2f;
        float lhsHalfHeight = lhsBounds.Height / 2f;
        float rhsHalfWidth = rhsBounds.Width / 2f;
        float rhsHalfHeight = rhsBounds.Height / 2f;

        // Calculate overlap on each axis
        float overlapX = lhsHalfWidth + rhsHalfWidth - Math.Abs(distance.X);
        float overlapY = lhsHalfHeight + rhsHalfHeight - Math.Abs(distance.Y);

        // The axis with the smallest overlap determines the normal
        if (overlapX < overlapY)
        {
            return new Vector2(Math.Sign(distance.X), 0);
        }
        else
        {
            return new Vector2(0, Math.Sign(distance.Y));
        }
    }
}
