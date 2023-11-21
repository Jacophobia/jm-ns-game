using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extensions;

internal static class VectorExtensions
{
    internal static IEnumerable<Vector2> GetAdjacentCoordinates(this Vector2 vector)
    {
        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
            yield return new Vector2(vector.X + x, vector.Y + y);
    }
}