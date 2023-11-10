using System;
using Microsoft.Xna.Framework;

namespace SpatialPartition.Collision;

internal static class CollisionDetector
{
    internal static bool TryRadialCollision(ICollidable collidable1, ICollidable collidable2, out Vector2? collisionLocation)
    {
        // Calculate the center of the bounding circles for both entities
        var center1 = new Vector2(collidable1.Destination.X + collidable1.Destination.Width / 2,
            collidable1.Destination.Y + collidable1.Destination.Height / 2);
        var center2 = new Vector2(collidable2.Destination.X + collidable2.Destination.Width / 2,
            collidable2.Destination.Y + collidable2.Destination.Height / 2);

        // Calculate the radius of the bounding circles for both entities (assuming they are circular)
        var radius1 = Math.Max(collidable1.Destination.Width, collidable1.Destination.Height) / 2f;
        var radius2 = Math.Max(collidable2.Destination.Width, collidable2.Destination.Height) / 2f;

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

    // internal static bool TryBoundingBoxCollision(ICollidable collidable1, ICollidable collidable2, out Rectangle? overlap)
    // {
    //     // Create rectangles for the entities' collision areas
    //     var rect1 = collidable1.Destination;
    //     var rect2 = collidable2.Destination;
    //
    //     // Find the intersection rectangle
    //     overlap = Rectangle.Intersect(rect1, rect2);
    //
    //     // Check if there is an intersection
    //     if (overlap is not { IsEmpty: true })
    //     {
    //         // Calculate the collision location as the center of the intersection rectangle
    //         return true;
    //     }
    //
    //     // No collision
    //     overlap = null;
    //     return false;
    // }

    // public static bool TryPixelPerfect(ICollidable collidable1, ICollidable collidable2, Rectangle overlap, out Vector2? collisionCoordinate)
    // {
    //     Debug.Assert(!overlap.IsEmpty, "Overlap cannot be empty. A value must be provided.");
    //
    //     // Create rectangles for the entities' collision areas
    //     var rect1 = collidable1.Destination;
    //     var rect2 = collidable2.Destination;
    //
    //     // Get the textures of the entities
    //     var texture1 = collidable1.Sprite;
    //     var texture2 = collidable2.Sprite;
    //
    //     // Get the pixel data from the textures
    //     var data1 = new Color[texture1.Width * texture1.Height];
    //     var data2 = new Color[texture2.Width * texture2.Height];
    //     texture1.GetData(data1);
    //     texture2.GetData(data2);
    //
    //     // Iterate through the intersection area and check for pixel-perfect collision
    //     for (var x = overlap.Left; x <= overlap.Right; x++)
    //     {
    //         for (var y = overlap.Top; y <= overlap.Bottom; y++)
    //         {
    //             // Calculate the pixel coordinates within the textures
    //             var texCoord1 = new Vector2((x - rect1.Left) * texture1.Width / rect1.Width,
    //                 (y - rect1.Top) * texture1.Height / rect1.Height);
    //             var texCoord2 = new Vector2((x - rect2.Left) * texture2.Width / rect2.Width,
    //                 (y - rect2.Top) * texture2.Height / rect2.Height);
    //
    //             // Get the pixel indices
    //             var index1 = (int)(texCoord1.X + texCoord1.Y * texture1.Width);
    //             var index2 = (int)(texCoord2.X + texCoord2.Y * texture2.Width);
    //
    //             // Check if the pixels at the current position are not transparent for both entities
    //             var pixel1 = data1[Math.Clamp(index1, 0, data1.Length - 1)];
    //             var pixel2 = data2[Math.Clamp(index2, 0, data2.Length - 1)];
    //
    //             if (pixel1.A == byte.MaxValue && pixel2.A == byte.MaxValue)
    //             {
    //                 // Calculate the collision point
    //                 collisionCoordinate = new Vector2(x, y);
    //                 return true;
    //             }
    //         }
    //     }
    //
    //     // No collision detected
    //     collisionCoordinate = null;
    //     return false;
    // }
}