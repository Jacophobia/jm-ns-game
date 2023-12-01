using System;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Interfaces;

namespace MonoGame.Decorators;

public class Rectangular : EntityDecorator
{
    private enum IntersectionSide
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }
    
    public Rectangular(Entity @base) : base(@base)
    {
    }
    
    private static Tuple<float?, float?> GetTimeToCollision(ICollidable lhs, ICollidable rhs)
    {
        // Calculate previous center positions
        var prevCenter1 = lhs.Position - lhs.Velocity;
        var prevCenter2 = rhs.Position - rhs.Velocity;

        // Calculate half dimensions for easier calculations
        var halfWidth1 = lhs.Bounds.Width / 2f;
        var halfHeight1 = lhs.Bounds.Height / 2f;
        var halfWidth2 = rhs.Bounds.Width / 2f;
        var halfHeight2 = rhs.Bounds.Height / 2f;

        // Calculate relative velocity
        var relativeVelocity = lhs.Velocity - rhs.Velocity;

        // Determine time to collision for each axis (if any)
        float? timeToCollisionX = null;
        float? timeToCollisionY = null;

        if (relativeVelocity.X != 0)
        {
            if (relativeVelocity.X > 0)
                timeToCollisionX = (prevCenter2.X - halfWidth2 - (prevCenter1.X + halfWidth1)) / relativeVelocity.X;
            else
                timeToCollisionX = (prevCenter2.X + halfWidth2 - (prevCenter1.X - halfWidth1)) / relativeVelocity.X;
        }

        if (relativeVelocity.Y != 0)
        {
            if (relativeVelocity.Y > 0)
                timeToCollisionY
                    = (prevCenter2.Y + halfHeight2 - (prevCenter1.Y - halfHeight1)) /
                      relativeVelocity.Y; // Inverted Y-axis
            else
                timeToCollisionY
                    = (prevCenter2.Y - halfHeight2 - (prevCenter1.Y + halfHeight1)) /
                      relativeVelocity.Y; // Inverted Y-axis
        }

        return new Tuple<float?, float?>(timeToCollisionX, timeToCollisionY);
    }

    private static IntersectionSide GetCollisionDirection(ICollidable lhs, ICollidable rhs)
    {
        var (timeToCollisionX, timeToCollisionY) = GetTimeToCollision(lhs, rhs);

        var timeX = timeToCollisionX ?? float.MaxValue;
        var timeY = timeToCollisionY ?? float.MaxValue;


        // Calculate relative velocity
        var relativeVelocity = lhs.Velocity - rhs.Velocity;

        // Determine the first side hit based on the smallest positive time to collision
        if (timeX >= 0 && timeX < timeY)
            return relativeVelocity.X > 0 ? IntersectionSide.Left : IntersectionSide.Right;
        if (timeY >= 0 && timeY < timeX)
            return relativeVelocity.Y > 0 ? IntersectionSide.Top : IntersectionSide.Bottom;

        return IntersectionSide.None;
    }

    protected override Vector2 OnCalculateCollisionNormal(ICollidable rhs, Vector2 collisionLocation)
    {
        if (IsStatic)
            return Vector2.Zero;

        return GetCollisionDirection(this, rhs) switch
        {
            IntersectionSide.Top => new Vector2(0, -1),
            IntersectionSide.Bottom => new Vector2(0, 1),
            IntersectionSide.Left => new Vector2(-1, 0),
            IntersectionSide.Right => new Vector2(1, 0),
            IntersectionSide.None => Vector2.Zero,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}