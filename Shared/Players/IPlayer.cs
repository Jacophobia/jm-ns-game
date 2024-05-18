using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Input;

namespace Shared.Players;

public interface IPlayer
{
    public Guid Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; }

    public void Follow(Func<Vector3> target);

    public void BeginDisplay();

    public void Display(Texture2D texture, Rectangle destination, float depth,
        Rectangle source, Color color, float rotation, Vector2 origin,
        SpriteEffects effect);
    
    public void Update(float deltaTime);

    public void EndDisplay();
}