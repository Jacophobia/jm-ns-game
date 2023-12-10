using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class External : Player
{
    private readonly NetworkHost _networkClient;

    public External(IPEndPoint endPoint, Camera perspective, NetworkHost networkClient) : base(endPoint, perspective, networkClient)
    {
        _networkClient = networkClient;
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