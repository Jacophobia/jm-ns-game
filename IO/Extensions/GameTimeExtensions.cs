using Microsoft.Xna.Framework;

namespace IO.Extensions;

public static class GameTimeExtensions
{
    public static float DeltaTime(this GameTime gameTime)
    {
        return (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}