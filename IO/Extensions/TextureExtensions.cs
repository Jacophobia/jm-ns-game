using Microsoft.Xna.Framework.Graphics;

namespace IO.Extensions;

public static class TextureExtensions
{
    public static string GetCollisionDataFilepath(this Texture2D texture)
    {
        return "Content\\Metadata\\" + texture.Name.Replace('/', '\\') + ".csv";
    }
}