using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Extensions;
using Shared.Input;

namespace Shared.Output;

public class Camera
{
    internal const float FocalLength = 10f;
    
    private readonly float _followSpeed;

    private readonly Stack<Func<Vector3>> _objectsToFollow;
    private readonly Vector3 _offset;
    private Vector3 _position;
    private Rectangle _view;

    public Camera(Func<Vector3> objectToFollow = null, float followSpeed = 1f, Vector3 offset = default)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;

        _followSpeed = followSpeed;
        _objectsToFollow = new Stack<Func<Vector3>>();
        _offset = offset;
        _position = offset;
        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);

        if (objectToFollow == null) 
            return;
        
        _position += objectToFollow();
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

    public void SetFocus(Func<Vector3> renderable)
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
        
        var offset = target() - _view.Center.ToVector3() - _offset;
        
        offset.Z = 0;
        
        if ((controls & Controls.Down) != 0)
        {
            offset += Vector3.Forward * 100;
        }
        if ((controls & Controls.Up) != 0)
        {
            offset += Vector3.Backward * 100;
        }

        offset *= _followSpeed * deltaTime;

        _position += offset;
    }
}