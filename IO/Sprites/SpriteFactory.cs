using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Sprites;

public class SpriteFactory
{
    private readonly ContentManager _contentManager;

    public SpriteFactory(ContentManager contentManager)
    {
        _contentManager = contentManager;
    }

    public Sprite Build(string name)
    {
        return new Sprite(_contentManager.Load<Texture2D>(name));
    }
}