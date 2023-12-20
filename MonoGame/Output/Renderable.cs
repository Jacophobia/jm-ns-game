using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Singletons;

namespace MonoGame.Output;

internal class Renderable : IRenderable
{
    public Texture2D Texture { get; set; }
    public string TextureName
    {
        set => Texture = GetTextureByName(value);
    }
    public Rectangle Destination { get; set; }
    public Rectangle Source { get; set; }
    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects Effect { get; set; }
    public float Depth { get; set; }

    void IRenderable.Render(IPlayer player)
    {
        player.Display(this);
    }

    // Method to retrieve Texture2D by its name
    private static Texture2D GetTextureByName(string name)
    {
        var textureManager = TextureManager.GetInstance();
        return textureManager[name];
    }
}
