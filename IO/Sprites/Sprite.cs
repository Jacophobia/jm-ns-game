using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Sprites;

public class Sprite
{
    public Texture2D Texture { get; }
    private readonly CollisionData _collisionData;

    public Sprite(Texture2D texture)
    {
        Texture = texture;
        _collisionData = new CollisionData(texture);
    }

    public static bool Overlaps(Rectangle lhsDestination, Rectangle rhsDestination, out Rectangle? overlap)
    {
        // Find the intersection rectangle
        overlap = Rectangle.Intersect(lhsDestination, rhsDestination);

        // Check if there is an intersection
        if (overlap is not { IsEmpty: true })
        {
            // Calculate the collision location as the center of the intersection rectangle
            return true;
        }

        // No collision
        overlap = null;
        return false;
    }

    public static bool Collides(Sprite lhs, Rectangle lhsDestination, Sprite rhs, Rectangle rhsDestination, Rectangle overlap, out Vector2? collisionCoordinate)
    {
        Debug.Assert(!overlap.IsEmpty, "Overlap cannot be empty. A value must be provided.");

        if ((lhsDestination.Width  < 11 && overlap.Width  >= lhsDestination.Width  / 9f) || 
            (lhsDestination.Height < 11 && overlap.Height >= lhsDestination.Height / 9f) || 
            (rhsDestination.Width  < 11 && overlap.Width  >= rhsDestination.Width  / 9f) || 
            (rhsDestination.Height < 11 && overlap.Height >= rhsDestination.Height / 9f))
        {
            collisionCoordinate = overlap.Center.ToVector2();
            return true;
        }

        // Create rectangles for the entities' collision areas
        var rect1 = lhsDestination;
        var rect2 = rhsDestination;

        // Get the textures of the entities
        var texture1 = lhs.Texture;
        var texture2 = rhs.Texture;

        // Iterate through the intersection area and check for pixel-perfect collision
        for (var x = overlap.Left; x <= overlap.Right; x++)
        {
            for (var y = overlap.Top; y <= overlap.Bottom; y++)
            {
                // Calculate the pixel coordinates within the textures
                var texCoord1 = new Vector2(
                    (int)Math.Round((float)((x - rect1.Left) * texture1.Width) / rect1.Width),
                    (int)Math.Round((float)((y - rect1.Top) * texture1.Height) / rect1.Height));
                var texCoord2 = new Vector2(
                    (int)Math.Round((float)((x - rect2.Left) * texture2.Width) / rect2.Width),
                    (int)Math.Round((float)((y - rect2.Top) * texture2.Height) / rect2.Height));

                // Check if the pixels at the current position are not transparent for both entities
                if (lhs._collisionData.IsCollidableCoord(texCoord1) && rhs._collisionData.IsCollidableCoord(texCoord2))
                {
                    // Calculate the collision point
                    collisionCoordinate = new Vector2(x, y);
                    return true;
                }
            }
        }

        // No collision detected
        collisionCoordinate = null;
        return false;
    }
}