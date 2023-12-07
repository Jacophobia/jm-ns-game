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
    private readonly Vector3 _offset;
    private Vector3 _position;
    private Rectangle _view;

    public Camera(IRenderable objectToFollow = null, float followSpeed = 1f, Vector3 offset = default)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;

        _followSpeed = followSpeed;
        _objectsToFollow = new Stack<IRenderable>();
        _offset = offset;
        _position = Vector3.Zero;
        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);

        if (objectToFollow == null) 
            return;
        
        var position = offset + objectToFollow.Destination.Center.ToVector3();
        _position = new Vector3(position.X, position.Y, 0);
        _objectsToFollow.Push(objectToFollow);
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
        if (!_objectsToFollow.TryPeek(out var target))
            return;
        
        var offset = (target.Destination.Center - _view.Center).ToVector3() - _offset;
        
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