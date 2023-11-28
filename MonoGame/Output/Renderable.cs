using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Singletons;

namespace MonoGame.Output;

internal class Renderable : IRenderable
{
    Texture2D IRenderable.Texture => Texture;
    internal Texture2D Texture { get; set; }

    internal string TextureName
    {
        get => Texture.Name;
        set => Texture = GetTextureByName(value);
    }

    Rectangle IRenderable.Destination
    {
        get => Destination;
        set => Destination = value;
    }
    internal Rectangle Destination { get; set; }

    Rectangle IRenderable.Source => Source;
    internal Rectangle Source { get; set; }

    Color IRenderable.Color => Color;
    internal Color Color { get; set; }

    float IRenderable.Rotation => Rotation;
    internal float Rotation { get; set; }

    Vector2 IRenderable.Origin => Origin;
    internal Vector2 Origin { get; set; }

    SpriteEffects IRenderable.Effect => Effect;
    internal SpriteEffects Effect { get; set; }

    int IRenderable.Depth => Depth;
    internal int Depth { get; set; }

    public Renderable()
    {
        
    }

    internal Renderable(string textureName, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effect, int depth)
    {
        // Assuming you have a way to get Texture2D from textureName
        TextureName = textureName;
        Destination = destination;
        Source = source;
        Color = color;
        Rotation = rotation;
        Origin = origin;
        Effect = effect;
        Depth = depth;
    }

    void IRenderable.Draw(Renderer renderer, Camera cameras)
    {
        renderer.Render(this, cameras);
    }

    // Method to retrieve Texture2D by its name
    private static Texture2D GetTextureByName(string name)
    {
        var textureManager = TextureManager.GetInstance();
        return textureManager[name];
    }
}
