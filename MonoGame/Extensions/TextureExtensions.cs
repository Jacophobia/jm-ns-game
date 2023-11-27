using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extensions;

internal static class TextureExtensions
{
    internal static string GetCollisionDataFilepath(this Texture2D texture)
    {
        return "Content\\Metadata\\" + texture.Name.Replace('/', '\\') + ".csv";
    }
}