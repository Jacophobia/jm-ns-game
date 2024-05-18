using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Players;
using OverWorld.GameObjects;

namespace OverWorld.GeneralActions;

public class Render : GeneralAction
{
    private readonly Player _player;
    
    public Render(Player player)
    {
        _player = player;
    }
    
    public override void Apply(IGameObject gameObject, float deltaTime)
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
}