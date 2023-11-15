using System;
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

    public override Rectangle Destination
    {
        get
        {
            _destination.X = (int)MathF.Round(Position.X);
            _destination.Y = (int)MathF.Round(Position.Y);
            return _destination;
        }
        set => _destination = value;
    }

    public override Rectangle Source { get; set; }
    public override Color Color { get; set; }
    public override float Rotation { get; set; }
    public override Vector2 Origin { get; set; }
    public override SpriteEffects Effect { get; set; }
    public override float Depth { get; set; }
    public override Sprite Sprite { get; set; }
    public override Vector2 Position { get; set; }
    public override Vector2 Velocity { get; set; }
    public override float RestitutionCoefficient { get; set; }
    public override bool IsStatic { get; set; }
    
    private Rectangle _destination;

    public override void Update(GameTime gameTime, Controls controls)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }
    
    public override void HandleCollisionFrom(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void HandleCollisionWith(ICollidable collidable, GameTime gameTime, Vector2? collisionLocation, Rectangle? overlap)
    {
        // We don't do anything. Entity behavior will be handled by the 
        //  decorators.
    }

    public override void Draw(Renderer renderer, Camera camera)
    {
        renderer.Render(this, camera);
    }
}