using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace client.Extensions;

public readonly struct Outline
{
    public Rectangle Top { get; init; }
    public Rectangle Bottom { get; init; }
    public Rectangle Left { get; init; }
    public Rectangle Right { get; init; }

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
        width ??= (rectangle.Width + rectangle.Height) / 20;

        return new Outline
        {
            Top = new Rectangle(rectangle.Left, rectangle.Top - width.Value, rectangle.Width, width.Value),
            Bottom = new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, width.Value),
            Right = new Rectangle(rectangle.Right, rectangle.Top - width.Value, width.Value, rectangle.Height + width.Value * 2),
            Left = new Rectangle(rectangle.Left - width.Value, rectangle.Top - width.Value, width.Value, rectangle.Height + width.Value * 2)
        };
    }
}