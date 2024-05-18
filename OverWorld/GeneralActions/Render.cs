using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OverWorld.GameObjects;
using Shared.Players;

namespace OverWorld.GeneralActions;

public class Render : GeneralAction
{
    private readonly IPlayer _player;
    
    public Render(IPlayer player)
    {
        _player = player;
    }

    public override void Begin()
    {
        _player.BeginDisplay();
    }

    public override void Apply(GameObject gameObject, float deltaTime)
    {
        _player.Display(
            gameObject.CurrentTexture,
            gameObject.Bounds, 
            gameObject.Layer, 
            gameObject.CurrentTexture.Bounds, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            SpriteEffects.None
        );
    }

    public override void End()
    {
        _player.EndDisplay();
    }
}