using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Sprites;

internal class SpriteFactory
{
    private readonly ContentManager _contentManager;

    internal SpriteFactory(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    internal Sprite Build(string name)
    {
        return new Sprite(_contentManager.Load<Texture2D>(name));
    }
}