using Microsoft.Xna.Framework.Graphics;

namespace Shared.Extensions;

internal static class TextureExtensions
{
    internal static string GetCollisionDataFilepath(this Texture2D texture)
    {
        return texture.Name.GetCollisionDataFilepath();
    }
    
    internal static string GetCollisionDataFilepath(this string textureName)
    {
        return "Content\\Metadata\\" + textureName.Replace('/', '\\') + ".csv";
    }
}