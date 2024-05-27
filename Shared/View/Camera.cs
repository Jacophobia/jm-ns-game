using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shared.Extensions;
using Shared.Updates;

namespace Shared.View;

public class Camera : IUpdatable
{
    private const float FocalLength = 10f;
    
    private readonly float _followSpeed;

    private readonly IViewable _target;
    private IViewable _tempTarget;
    private TimeSpan _remainingTempTargetTime;
    private readonly Vector3 _offset;
    private Vector3 _position;
    private Rectangle _view;

    public Camera(IViewable objectToFollow, float followSpeed = 1f, Vector3? offset = null)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;

        _followSpeed = followSpeed;
        offset ??= Vector3.Backward * FocalLength;
        _offset = offset.Value;
        _position = offset.Value;
        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);

        if (objectToFollow == null) 
            return;
        
        _position += objectToFollow.CurrentPosition;
        _target = objectToFollow;
        _tempTarget = null;
        _remainingTempTargetTime = TimeSpan.Zero;
    }

    private float Depth => _position.Z;

    private Rectangle View
    {
        get
        {
            _view.X = (int)MathF.Round(_position.X + _offset.X);
            _view.Y = (int)MathF.Round(_position.Y + _offset.Y);
            return _view;
        }
    }

    public void SetTemporaryFocus(IViewable renderable, TimeSpan duration)
    {
        _tempTarget = renderable;
        _remainingTempTargetTime = duration;
    }

    public bool CanSee(Rectangle rectangle, float depth)
    {
        return rectangle.Intersects(View) || Depth < depth;
    }

    public void Adjust(ref Rectangle rectangle)
    {
        rectangle.X -= View.X;
        rectangle.Y -= View.Y;
    }

    public void Update(GameTime gameTime)
    {
        if (_remainingTempTargetTime > TimeSpan.Zero)
        {
            _remainingTempTargetTime -= gameTime.ElapsedGameTime;
        }
        
        if (_remainingTempTargetTime <= TimeSpan.Zero && _tempTarget != null)
        {
            _tempTarget = null;
        }
        
        var offset = (_tempTarget ?? _target).CurrentPosition - _view.Center.ToVector3() - _offset;
        
        offset.Z = 0;

        offset *= _followSpeed * gameTime.DeltaTime();

        _position += offset;
    }
}