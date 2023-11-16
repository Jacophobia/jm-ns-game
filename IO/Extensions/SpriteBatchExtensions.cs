using IO.Interfaces;
using IO.Output;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Extensions;

public static class SpriteBatchExtensions
{
    public static void Draw(this SpriteBatch spriteBatch, IRenderable renderable, Camera camera)
    {
        var relativeDestination = new Rectangle(
            renderable.Destination.X - camera.View.X,
            renderable.Destination.Y - camera.View.Y,
            renderable.Destination.Width,
            renderable.Destination.Height
        );

        spriteBatch.Draw(
            renderable.Texture,
            relativeDestination,
            renderable.Source,
            renderable.Color,
            renderable.Rotation,
            renderable.Origin,
            renderable.Effect,
            renderable.Depth
        );
    }

    public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Color color,
        Camera camera)
    {
        var relativeDestination = new Rectangle(
            destination.X - camera.View.X,
            destination.Y - camera.View.Y,
            destination.Width,
            destination.Height
        );

        spriteBatch.Draw(texture, relativeDestination, color);
    }
}