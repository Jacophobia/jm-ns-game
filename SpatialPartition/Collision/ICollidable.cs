using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpatialPartition.Collision;

public interface ICollidable
{
    public Texture2D Texture { get; }
    public Rectangle Destination { get; }
    public Vector2 Position { get; }
    public bool CollidesWith(ICollidable rhs, out Vector2 collisionLocation);
}