using Microsoft.Xna.Framework;

namespace MonoGame.Extensions;

public static class RectangleExtensions
{
    public static float Mass(this Rectangle rectangle) => rectangle.Width * rectangle.Height;
}