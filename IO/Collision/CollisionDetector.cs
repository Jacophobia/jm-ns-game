using System;
using jm_ns_game.Interfaces;
using Microsoft.Xna.Framework;

namespace jm_ns_game.Collision;

public static class CollisionDetector
{
    public static bool TryRadialCollision(IRenderable entity1, IRenderable entity2, out Vector2? collisionLocation)
    {
        // Calculate the center of the bounding circles for both entities
        var center1 = new Vector2(entity1.Destination.X + entity1.Destination.Width / 2, entity1.Destination.Y + entity1.Destination.Height / 2);
        var center2 = new Vector2(entity2.Destination.X + entity2.Destination.Width / 2, entity2.Destination.Y + entity2.Destination.Height / 2);

        // Calculate the radius of the bounding circles for both entities (assuming they are circular)
        var radius1 = Math.Max(entity1.Destination.Width, entity1.Destination.Height) / 2f;
        var radius2 = Math.Max(entity2.Destination.Width, entity2.Destination.Height) / 2f;

        // Calculate the distance between the centers of the bounding circles
        var distance = Vector2.Distance(center1, center2);

        // Check for collision based on the radii and distance
        if (distance <= radius1 + radius2)
        {
            // Calculate the collision location as the point of intersection
            collisionLocation = Vector2.Lerp(center1, center2, radius1 / (radius1 + radius2));
            return true;
        }

        // No collision
        collisionLocation = null;
        return false;
    }

    public static bool TryBoundingBoxCollision(IRenderable entity1, IRenderable entity2, out Vector2? collisionLocation)
    {
        // Create rectangles for the entities' collision areas
        var rect1 = entity1.Destination;
        var rect2 = entity2.Destination;

        // Check for collision based on bounding boxes
        if (rect1.Intersects(rect2))
        {
            // Calculate the collision location as the point of intersection
            collisionLocation = new Vector2(Math.Max(rect1.Left, rect2.Left), Math.Max(rect1.Top, rect2.Top));
            return true;
        }

        // No collision
        collisionLocation = null;
        return false;
    }

    
    public static bool TryPixelPerfect(IRenderable entity1, IRenderable entity2, out Vector2? collisionCoordinate)
    {
        // Create rectangles for the entities' collision areas
        var rect1 = entity1.Destination;
        var rect2 = entity2.Destination;

        // Calculate the intersection rectangle
        var intersection = Rectangle.Intersect(rect1, rect2);

        // Check if there is a collision
        if (intersection.IsEmpty)
        {
            collisionCoordinate = null;
            return false;
        }
        
        // Get the textures of the entities
        var texture1 = entity1.Texture;
        var texture2 = entity2.Texture;

        // Create arrays to hold the pixel data of the textures
        var data1 = new Color[texture1.Width * texture1.Height];
        var data2 = new Color[texture2.Width * texture2.Height];

        // Get the pixel data from the textures
        texture1.GetData(data1);
        texture2.GetData(data2);

        // Calculate the relative position of the intersection rectangle within the entities
        var relativePosition = new Vector2(intersection.X - rect1.X, intersection.Y - rect1.Y);

        // Iterate through the intersection area and check for pixel-perfect collision
        for (var x = 0; x < intersection.Width; x++)
        {
            for (var y = 0; y < intersection.Height; y++)
            {
                var index1 = (int)(relativePosition.Y + y) * rect1.Width + (int)(relativePosition.X + x);
                var index2 = (int)(intersection.Y - rect2.Y + y) * rect2.Width + (int)(intersection.X - rect2.X + x);

                // Check if the pixels at the current position are not transparent for both entities
                if (data1[index1].A == 0 || data2[index2].A == 0) continue;
                // Calculate the collision point relative to entity1
                collisionCoordinate = new Vector2(rect1.X + relativePosition.X + x, rect1.Y + relativePosition.Y + y);
                return true;
            }
        }

        // No collision detected
        collisionCoordinate = null;
        return false;
    }
}


