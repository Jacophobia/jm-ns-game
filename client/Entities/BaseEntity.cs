using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.Entities;

public sealed class BaseEntity : Entity
{
    public override Texture2D Texture { get; set; }
    public override Rectangle Destination { get; set; }
    public override Rectangle Source { get; set; }
    public override Color Color { get; set; }
    public override float Rotation { get; set; }
    public override Vector2 Origin { get; set; }
    public override SpriteEffects Effect { get; set; }
    public override float Depth { get; set; }
    public override Vector2 Velocity { get; set; }

    public override void Update(GameTime _)
    {
        // We don't do anything. Update behavior will be handled by the 
        //  decorators.
    }
}