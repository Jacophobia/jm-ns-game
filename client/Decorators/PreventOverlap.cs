using System;
using System.Diagnostics;
using client.Entities;
using IO.Extensions;
using IO.Input;
using IO.Output;
using IO.Sprites;
using Microsoft.Xna.Framework;
using SpatialPartition.Collision;

namespace client.Decorators;

public class PreventOverlap : EntityDecorator
{
    public PreventOverlap(Entity @base) : base(@base)
    {
        // no new behavior to add
    }

    protected override void OnUpdate(GameTime gameTime, Controls controls)
    {
        // no new behavior to add
    }

    protected override void OnHandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        
    }

    protected override void OnHandleCollisionWith(ICollidable rhs, GameTime gameTime, Vector2? collisionLocation,
        Rectangle? overlap)
    {
        Debug.Assert(collisionLocation != null, "This method should not be called if collisionLocation is null");
        Debug.Assert(overlap != null, "This method should not be called if overlap is null");

        if (IsStatic && rhs.IsStatic)
            return;
        
        const int maxTries = 1000;
        var tries = 0;
        while (tries < maxTries)
        {
            var previousLhsPosition = Position + -Velocity * gameTime.DeltaTime();
            var previousRhsPosition = rhs.Position + -rhs.Velocity * gameTime.DeltaTime();
        
            var previousLhsDestination = new Rectangle((int)previousLhsPosition.X, (int)previousLhsPosition.Y, Destination.Width, Destination.Height);
            var previousRhsDestination = new Rectangle((int)previousRhsPosition.X, (int)previousRhsPosition.Y, rhs.Destination.Width, rhs.Destination.Height);

            if (!IsStatic)
                Position = previousLhsPosition;
            if (!rhs.IsStatic )
                rhs.Position = previousRhsPosition;
            
            if (!Sprite.Overlaps(previousLhsDestination, previousRhsDestination, out var newOverlap)
                || !newOverlap.HasValue
                || !Sprite.Collides(Sprite, previousLhsDestination, rhs.Sprite, previousRhsDestination, newOverlap.Value,
                    out var coordinate))
            {
                tries = maxTries;
            }

            tries++;
        }
        // if they are separating then don't stop them



        // calculate the previous position of each ICollidable
        // see if their paths intersected. Width & height need to be 
        // into account
        // if they did, change their position so that they are right
        // where they were before they collided.
    }


    protected override void OnDraw(Camera camera)
    {
        // no new behavior to add
    }
}