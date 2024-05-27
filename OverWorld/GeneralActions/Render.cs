using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OverWorld.GameObjects;
using Shared.Rendering;
using Shared.View;

namespace OverWorld.GeneralActions;

public class Render : GeneralAction
{
    private readonly IRenderer _renderer;
    private readonly Camera _camera;
    
    public Render(IRenderer renderer, Camera camera)
    {
        _renderer = renderer;
        _camera = camera;
    }

    public override void Begin()
    {
        _renderer.Begin();
    }

    public override void Apply(GameObject gameObject, GameTime gameTime)
    {
        _renderer.Render(
            _camera,
            gameObject.CurrentTexture,
            gameObject.Bounds, 
            gameObject.CurrentTexture.Bounds, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            SpriteEffects.None,
            gameObject.Layer
        );
    }

    public override void End()
    {
        _renderer.End();
    }
}