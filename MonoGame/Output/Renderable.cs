using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Players;
using MonoGame.Singletons;

namespace MonoGame.Output;

internal class Renderable : IRenderable
{

    public Renderable()
    {
        
    }

    internal Renderable(string textureName, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effect, int depth)
    {
        TextureName = textureName;
        Destination = destination;
        Source = source;
        Color = color;
        Rotation = rotation;
        Origin = origin;
        Effect = effect;
        Depth = depth;
    }

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
    public int Depth { get; set; }

    void IRenderable.Draw(Player player)
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
