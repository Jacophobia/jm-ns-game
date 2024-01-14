using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Decorators;
using MonoGame.Entities;

namespace client.Entities;

public static class Characters
{
    private static string DefaultTextureName => "Test/character-neutral-idle";
    private static Vector2 Position => new (2560f / 2f, 1440f / 2f);
    private static Vector2 Velocity => Vector2.Zero;
    private static float Scale => 1f;
    private static Color Color => Color.White;
    private static int Depth => 0;
    private static float Friction => 0.01f;
    private static float Jitter => 0.125f;
    private static bool PerspectiveScale => true;
    
    public static EntityBuilder Default(string textureName = null, Vector2? position = null, Vector2? velocity = null,
        float? scale = null, Rectangle? source = null, Color? color = null, float? rotation = null,
        Vector2? origin = null, SpriteEffects? effect = null, int? depth = null, bool isStatic = false)
    {
        return new EntityBuilder(
                textureName ?? DefaultTextureName,
                position ?? Position,
                velocity ?? Velocity,
                scale: scale ?? Scale,
                source: source,
                color: color ?? Color,
                rotation: rotation,
                origin: origin,
                effect: effect,
                depth: depth ?? Depth,
                isStatic: isStatic)
            .Add<Friction>(Friction)
            .Add<RemoveJitter>(Jitter)
            .Add<Inertia>()
            .Add<Collision>()
            // .AddDecorator<Drag>(20f)
            .Add<Rectangular>()
            // .AddDecorator<Circular>()
            .Add<Gravity>()
            // .AddDecorator<Bound>(new Rectangle(-2560 / 2, -1440 / 2, 2560 * 2, 1440 * 2))
            .Add<PerspectiveRender>(PerspectiveScale);
    }
}