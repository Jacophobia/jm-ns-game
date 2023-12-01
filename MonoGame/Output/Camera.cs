using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Output;

public class Camera
{
    private readonly float _followSpeed;

    private readonly Stack<IRenderable> _objectsToFollow;
    private readonly Vector3 _offset;
    private Vector3 _position;
    private Rectangle _view;

    public Camera(IRenderable objectToFollow, float followSpeed, Vector3 offset, int clientIndex = 0)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;
        var position = offset + objectToFollow.Destination.Center.ToVector3();

        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);
        _position = new Vector3(position.X + displayMode.Width / 2f, position.Y + displayMode.Height / 2f, -10f);
        _objectsToFollow = new Stack<IRenderable>();
        _objectsToFollow.Push(objectToFollow);
        _followSpeed = followSpeed;
        _offset = offset;
        ClientIndex = clientIndex;
    }

    internal Vector3 Position => _position;
    internal int ClientIndex { get; }

    internal Rectangle View
    {
        get
        {
            _view.X = (int)MathF.Round(_position.X + _offset.X);
            _view.Y = (int)MathF.Round(_position.Y + _offset.Y);
            return _view;
        }
    }

    public void SetFocus(IRenderable renderable)
    {
        _objectsToFollow.Push(renderable);
    }

    public void RevertFocus()
    {
        if (_objectsToFollow.Count > 1)
            _objectsToFollow.Pop();
    }

    public void Update(float deltaTime, IList<Controls> controls)
    {
        _position.X += (_objectsToFollow.Peek().Destination.Center.X - _offset.X - _view.Center.X)
                       * (_followSpeed * deltaTime);
        _position.Y += (_objectsToFollow.Peek().Destination.Center.Y - _offset.Y - _view.Center.Y)
                       * (_followSpeed * deltaTime);
    }
}