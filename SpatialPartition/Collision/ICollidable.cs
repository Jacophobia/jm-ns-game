using IO.Sprites;
using Microsoft.Xna.Framework;

namespace SpatialPartition.Collision;

public interface ICollidable
{
    public Sprite Sprite { get; }
    public Rectangle Destination { get; }
    public int Width => Destination.Width;
    public int Height => Destination.Height;
    public void Update(GameTime gameTime);
    public void HandleCollisionWith(ICollidable collidable, Vector2? collisionLocation);

    public bool CollidesWith(ICollidable rhs, out Vector2? collisionLocation)
    {
        if (Sprite.Overlaps(Destination, rhs.Destination, out var overlap) && overlap.HasValue)
        {
            return Sprite.Collides(Sprite, Destination, rhs.Sprite, rhs.Destination, overlap.Value, out collisionLocation);
        }
        
        collisionLocation = null;
        return false;
    }
}