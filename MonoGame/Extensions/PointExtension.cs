using Microsoft.Xna.Framework;

namespace MonoGame.Extensions;

internal static class PointExtension
{
    internal static Vector3 ToVector3(this Point point)
    {
        var vec3 = Vector3.Zero;

        vec3.X = point.X;
        vec3.Y = point.Y;

        return vec3;
    }
}