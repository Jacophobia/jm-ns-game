using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Remote : Player
{
    private readonly NetworkClient _networkClient;
    private string _playerId;

    public Remote(Camera perspective, NetworkClient networkClient) : base(perspective)
    {
        _networkClient = networkClient;
        _playerId = "";
    }

    public override void BeginDisplay()
    {
        _networkClient.PrepareRenderableBatch();
    }

    protected override void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null)
    {
        _networkClient.Enqueue(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public override void EndDisplay()
    {
        _networkClient.SendRenderableBatch();
    }
}