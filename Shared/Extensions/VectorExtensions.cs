using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Shared.Extensions;

internal static class VectorExtensions
{
    internal static IEnumerable<Vector2> GetAdjacentCoordinates(this Vector2 vector)
    {
        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
            yield return new Vector2(vector.X + x, vector.Y + y);
    }
    
    internal static Vector3 ToVector3(this Point point)
    {
        var vec3 = Vector3.Zero;

        vec3.X = point.X;
        vec3.Y = point.Y;

        return vec3;
    }
}