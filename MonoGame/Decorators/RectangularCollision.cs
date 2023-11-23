using System;
using Microsoft.Xna.Framework;
using MonoGame.Entities;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Decorators;

public class RectangularCollision : EntityDecorator
{
    private enum IntersectionSide
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }

    public RectangularCollision(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls[] controls)
    {
        // no new behavior to add
    }

    private static Tuple<float?, float?> GetTimeToCollision(ICollidable lhs, ICollidable rhs)
    {
        // Calculate previous center positions
        var prevCenter1 = lhs.Position - lhs.Velocity;
        var prevCenter2 = rhs.Position - rhs.Velocity;

        // Calculate half dimensions for easier calculations
        var halfWidth1 = lhs.Destination.Width / 2f;
        var halfHeight1 = lhs.Destination.Height / 2f;
        var halfWidth2 = rhs.Destination.Width / 2f;
        var halfHeight2 = rhs.Destination.Height / 2f;

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
    } // ReSharper disable twice PossibleInvalidOperationException
    private static float? GetMinimumPositiveTime(float? timeX, float? timeY)
    {
        return timeY switch
        {
            > 0 when timeX is null or < 0 => timeY,
            null or < 0 when timeX is > 0 => timeX,
            null or < 0 when timeX is null or < 0 => null,
            _ => Math.Min(timeX.Value, timeY.Value)
        };
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


    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        var (timeX, timeY) = GetTimeToCollision(this, rhs);

        var minTime = GetMinimumPositiveTime(timeX, timeY);

        if (minTime.HasValue)
        {
            Position -= Velocity * minTime.Value * gameTime.DeltaTime();
            rhs.Position -= rhs.Velocity * minTime.Value * gameTime.DeltaTime();
        }

        var lhsVelocity = Velocity;
        var rhsVelocity = rhs.Velocity;

        switch (GetCollisionDirection(this, rhs))
        {
            case IntersectionSide.Top:
                lhsVelocity.Y = Math.Abs(lhsVelocity.Y) * -1;
                rhsVelocity.Y = Math.Abs(rhsVelocity.Y);
                break;
            case IntersectionSide.Bottom:
                lhsVelocity.Y = Math.Abs(lhsVelocity.Y);
                rhsVelocity.Y = Math.Abs(rhsVelocity.Y) * -1;
                break;
            case IntersectionSide.Left:
                lhsVelocity.X = Math.Abs(lhsVelocity.X) * -1;
                rhsVelocity.X = Math.Abs(rhsVelocity.X);
                break;
            case IntersectionSide.Right:
                lhsVelocity.X = Math.Abs(lhsVelocity.X);
                rhsVelocity.X = Math.Abs(rhsVelocity.X) * -1;
                break;
        }

        Velocity = lhsVelocity;
        rhs.Velocity = rhsVelocity;
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
}