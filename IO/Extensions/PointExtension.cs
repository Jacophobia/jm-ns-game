using Microsoft.Xna.Framework;

namespace IO.Extensions;

public static class PointExtension
{
    public static Vector3 ToVector3(this Point point)
    {
        var vec3 = Vector3.Zero;

        vec3.X = point.X;
        vec3.Y = point.Y;

        return vec3;
    }
}