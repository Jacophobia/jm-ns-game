using Microsoft.Xna.Framework;

namespace MonoGame.Extensions;

internal static class GameTimeExtensions
{
    internal static float DeltaTime(this GameTime gameTime)
    {
        return (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}