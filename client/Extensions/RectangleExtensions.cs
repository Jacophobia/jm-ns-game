using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace client.Extensions;

public struct Outline
{
    public Rectangle Top { get; set; }
    public Rectangle Bottom { get; set; }
    public Rectangle Left { get; set; }
    public Rectangle Right { get; set; }

    public IEnumerable<Rectangle> GetSides()
    {
        yield return Top;
        yield return Bottom;
        yield return Left;
        yield return Right;
    }
}

public static class RectangleExtensions
{
    public static Outline GetOutline(this Rectangle rectangle, int? width = null)
    {
        var thickness = width ?? (rectangle.Width + rectangle.Height) / 20;
        var halfThickness = thickness / 2;

        return new Outline
        {
            Top = new Rectangle(rectangle.Center.X, rectangle.Top - halfThickness, rectangle.Width, thickness),
            Bottom = new Rectangle(rectangle.Center.X, rectangle.Bottom + halfThickness, rectangle.Width, thickness),
            Right = new Rectangle(rectangle.Right + halfThickness, rectangle.Center.Y, thickness, rectangle.Height),
            Left = new Rectangle(rectangle.Left - halfThickness, rectangle.Center.Y, thickness, rectangle.Height)
        };
    }
}