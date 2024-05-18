using Microsoft.Xna.Framework;

namespace OverWorld.Extensions;

public static class VectorExtensions
{
    internal static Point ToPoint(this Vector2 vector)
    {
        vector.Round();
        return new Point((int)vector.X, (int)vector.Y);
    }
}