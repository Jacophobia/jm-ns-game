using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Singletons;

namespace MonoGame.Output;

internal class Renderable : IRenderable
{
    private readonly Texture2D _texture;
    private Rectangle _destination;
    private readonly Rectangle _source;
    private readonly Color _color;
    private readonly float _rotation;
    private readonly Vector2 _origin;
    private readonly SpriteEffects _effect;
    private readonly int _depth;

    Texture2D IRenderable.Texture => _texture;
    Rectangle IRenderable.Destination
    {
        get => _destination;
        set => _destination = value;
    }
    Rectangle IRenderable.Source => _source;
    Color IRenderable.Color => _color;
    float IRenderable.Rotation => _rotation;
    Vector2 IRenderable.Origin => _origin;
    SpriteEffects IRenderable.Effect => _effect;
    int IRenderable.Depth => _depth;

    internal Renderable(string textureName, Rectangle destination, Rectangle source, Color color, float rotation, Vector2 origin, SpriteEffects effect, int depth)
    {
        // Assuming you have a way to get Texture2D from textureName
        _texture = GetTextureByName(textureName);
        _destination = destination;
        _source = source;
        _color = color;
        _rotation = rotation;
        _origin = origin;
        _effect = effect;
        _depth = depth;
    }

    void IRenderable.Draw(Renderer renderer, Camera camera)
    {
        renderer.Render(this, camera);
    }

    // Method to retrieve Texture2D by its name
    private static Texture2D GetTextureByName(string name)
    {
        var textureManager = TextureManager.GetInstance();
        return textureManager[name];
    }
}
