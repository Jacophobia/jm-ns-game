using IO.Input;
using IO.Interfaces;
using IO.Sprites;
using Microsoft.Xna.Framework;

namespace SpatialPartition.Collision;

public interface ICollidable : IRenderable
{
    public Sprite Sprite { get; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float RestitutionCoefficient { get; set; }
    public bool IsStatic { get; set; }
    public int Mass { get; }
    public void Update(GameTime gameTime, Controls controls);
    public void HandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap);
    public void HandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap);

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