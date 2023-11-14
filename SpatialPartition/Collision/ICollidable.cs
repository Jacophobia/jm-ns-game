using IO.Input;
using IO.Interfaces;
using IO.Sprites;
using Microsoft.Xna.Framework;

namespace SpatialPartition.Collision;

public interface ICollidable : IRenderable
{
    public Sprite Sprite { get; }
    public Vector2 Velocity { get; set; }
    public void Update(GameTime gameTime, Controls controls);
    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation, Rectangle? overlap);

    public bool CollidesWith(ICollidable rhs, out Vector2? collisionLocation, out Rectangle? overlap)
    {
        if (Sprite.Overlaps(Destination, rhs.Destination, out overlap) && overlap.HasValue)
        {
            return Sprite.Collides(Sprite, Destination, rhs.Sprite, rhs.Destination, overlap.Value, out collisionLocation);
        }
        
        collisionLocation = null;
        return false;
    }
}