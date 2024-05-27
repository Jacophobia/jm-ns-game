using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.View;

namespace Shared.Rendering;

public interface IRenderer
{
    void Begin();
    void Render(Texture2D texture, Rectangle destination, Rectangle source, 
        Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth);
    void Render(Camera camera, Texture2D texture, Rectangle destination, Rectangle source,
        Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth);
    void End();
}