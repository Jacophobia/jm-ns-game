using Microsoft.Xna.Framework;

namespace Shared.Extensions;

public static class RectangleExtensions
{
    public static float Mass(this Rectangle rectangle) => rectangle.Width * rectangle.Height;
}