using IO.Input;
using IO.Output;
using IO.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpatialPartition.Collision;

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
    public override Sprite Sprite { get; set; }
    public override Vector2 Velocity { get; set; }

    public override void Update(GameTime gameTime, Controls controls)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void Draw(Camera camera)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }
}