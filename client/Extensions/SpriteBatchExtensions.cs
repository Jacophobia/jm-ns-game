using client.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace client.Extensions;

internal static class SpriteBatchExtensions
{
    internal static void Draw(this SpriteBatch spriteBatch, IRenderable renderable)
    {
        spriteBatch.Draw(
            renderable.Texture,
            renderable.Destination,
            renderable.Source,
            renderable.Color,
            renderable.Rotation,
            renderable.Origin,
            renderable.Effect,
            renderable.Depth
        );
    }
}