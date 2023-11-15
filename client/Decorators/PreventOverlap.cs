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