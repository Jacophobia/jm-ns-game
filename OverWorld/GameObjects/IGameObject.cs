using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OverWorld.Physics;

namespace OverWorld.GameObjects;

public interface IGameObject
{
    // modifiable
    public Point Position { get; set; }
    public int Layer { get; set; }
    public Movement Movement { get; set; }
    
    // unmodifiable
    public Texture2D CurrentTexture { get; }
    public Rectangle Bounds { get; }
    
    // methods
    public void Update(float deltaTime);
}