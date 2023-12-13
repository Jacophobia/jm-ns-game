using System;
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

    protected override void OnDisplay(IWritable writable)
    {
        // If we want to support external menus (which we will for the
        // pause menu and the inventory menu) then we will need to
        // implement a method in NetworkHost / Server that supports
        // displaying text
        throw new NotImplementedException(); 
    }
    
    public override void EndDisplay()
    {
        _networkClient.SendRenderableBatch();
    }
}