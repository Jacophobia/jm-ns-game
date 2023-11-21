using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Output;

public class Renderable : IRenderable
{
    public Texture2D Texture { get; }
    public Rectangle Destination { get; set; }
    public Rectangle Source { get; }
    public Color Color { get; }
    public float Rotation { get; }
    public Vector2 Origin { get; }
    public SpriteEffects Effect { get; }
    public int Depth { get; }

    public Renderable(string textureName, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effect, int depth)
    {
        // Assuming you have a way to get Texture2D from textureName
        // Texture = GetTextureByName(textureName);
        Destination = destination;
        Source = source;
        Color = color;
        Rotation = rotation;
        Origin = origin;
        Effect = effect;
        Depth = depth;
    }
    
    public void Draw(Renderer renderer, Camera camera)
    {
        throw new System.NotImplementedException();
    }

    // Method to retrieve Texture2D by its name
    // private Texture2D GetTextureByName(string name)
    // {
    //     // Implementation to retrieve Texture2D based on name
    // }
}
