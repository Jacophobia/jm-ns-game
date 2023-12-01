using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Remote : Player
{
    private NetworkClient _networkClient;
    private string _playerId;

    public Remote(Camera camera, NetworkClient networkClient) : base(camera)
    {
        _networkClient = networkClient;
        _playerId = "";
    }

    // internal void Send(IRenderable renderable, Texture2D texture = null, 
    //     Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
    //     float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
    //     int? depth = null)
    // {
    //     var sendableRenderable = _renderablePool.Get();
    //     
    //     sendableRenderable.Texture = texture ?? renderable.Texture;
    //     sendableRenderable.Destination = destination ?? renderable.Destination;
    //     sendableRenderable.Source = source ?? renderable.Source;
    //     sendableRenderable.Color = color ?? renderable.Color;
    //     sendableRenderable.Rotation = rotation ?? renderable.Rotation;
    //     sendableRenderable.Origin = origin ?? renderable.Origin;
    //     sendableRenderable.Effect = effect == SpriteEffects.None ? renderable.Effect : effect;
    //     sendableRenderable.Depth = depth ?? renderable.Depth;
    //     
    //     _networkRenderables.Put(sendableRenderable, _networkClient.TotalMilliseconds);
    // }
    //
    // private void SendAllRenderables()
    // {
    //     if (_networkRenderables.IsEmpty) return;
    //     
    //     var renderables = _networkRenderables.GetAll().ToArray();
    //     _networkClient.SendRenderableData(renderables);
    //     foreach (var renderable in renderables)
    //         _renderablePool.Return(renderable);
    // }

    protected override void OnDisplay(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, int? depth = null)
    {
        throw new System.NotImplementedException();
    }
}