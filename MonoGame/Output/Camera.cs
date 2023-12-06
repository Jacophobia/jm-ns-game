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
    internal const float FocalLength = 10f;
    
    private readonly float _followSpeed;

    private readonly Stack<IRenderable> _objectsToFollow;
    private Vector3 _offset;
    private Vector3 _position;
    private Rectangle _view;

    public Camera(IRenderable objectToFollow, float followSpeed, Vector3 offset)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;
        var position = offset + objectToFollow.Destination.Center.ToVector3();

        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);
        _position = new Vector3(position.X + displayMode.Width / 2f, position.Y + displayMode.Height / 2f, 0);
        _objectsToFollow = new Stack<IRenderable>();
        _objectsToFollow.Push(objectToFollow);
        _followSpeed = followSpeed;
        _offset = offset;
    }

    internal float Depth => _position.Z;

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

    // ReSharper disable once ConvertIfStatementToSwitchStatement
    public void Update(float deltaTime, Controls controls)
    {
        var offset = (_objectsToFollow.Peek().Destination.Center - _view.Center).ToVector3() - _offset;
        offset.Z = 0;
        
        if (controls == Controls.Down)
        {
            offset += Vector3.Forward * 100;
        }
        if (controls == Controls.Up)
        {
            offset += Vector3.Backward * 100;
        }

        offset *= _followSpeed * deltaTime;

        _position += offset;
    }
}