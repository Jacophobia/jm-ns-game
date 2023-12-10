using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Players;

public class Remote : IPlayer
{
    private readonly Renderer _renderer;
    private readonly NetworkClient _networkClient;
    private readonly Listener _listener;
    private readonly List<IUpdatable> _updatables;

    public Remote(string id, Renderer renderer, Rectangle perspective, NetworkClient networkClient, float depth = -10, float focalLength = Camera.FocalLength)
    {
        _renderer = renderer;
        Perspective = perspective;
        Depth = depth;
        FocalLength = focalLength;
        _networkClient = networkClient;
        Id = new PlayerId(id);
        _listener = new Listener(new Dictionary<Keys, Controls>
        {
            { Keys.A, Controls.Left },
            { Keys.E, Controls.Right },
            { Keys.OemComma, Controls.Up },
            { Keys.O, Controls.Down },
            { Keys.X, Controls.Jump }
        });
        _updatables = new List<IUpdatable>();
    }

    public PlayerId Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; private set; }

    public void Follow(IRenderable target)
    {
        throw new System.NotImplementedException(); // currently we don't have a need for this since the client camera doesn't move
    }

    public void BeginDisplay()
    { 
        _renderer.Begin();
    }

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null,
        Color? color = null, float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None,
        float? depth = null)
    {
        _renderer.Draw(renderable, texture, destination, source, color, rotation, origin, effect, depth);
    }

    public void Update(float deltaTime)
    {
        Controls = _listener.GetControls(this);
        
        _networkClient.SendControlData(Controls);

        foreach (var updatable in _updatables)
        {
            updatable.Update(deltaTime);
        }
    }

    public void Add(IUpdatable updatable)
    {
        _updatables.Add(updatable);
    }

    public void Remove(IUpdatable updatable)
    {
        _updatables.Remove(updatable);
    }

    public void EndDisplay()
    {
        _renderer.End();
    }
}