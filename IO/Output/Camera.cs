using System;
using System.Collections.Generic;
using System.Linq;
using IO.Extensions;
using IO.Input;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Output;

public class Camera
{
    private readonly float _followSpeed;

    private readonly IList<IRenderable> _objectsToFollow;
    private readonly Vector3 _offset;
    private int _currentObject;
    private Vector2 _position;
    private Rectangle _view;

    public Camera(IRenderable objectToFollow, float followSpeed, Vector3 offset)
        : this(new List<IRenderable> { objectToFollow }, followSpeed, offset)
    {
    }

    public Camera(IList<IRenderable> objectsToFollow, float followSpeed, Vector3 offset)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;
        var position = offset + objectsToFollow.First().Destination.Center.ToVector3();

        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);
        _position = new Vector2(position.X + displayMode.Width / 2f, position.Y + displayMode.Height / 2f);
        _objectsToFollow = objectsToFollow;
        _followSpeed = followSpeed;
        _offset = offset;
    }

    public Rectangle View
    {
        get
        {
            _view.X = (int)MathF.Round(_position.X);
            _view.Y = (int)MathF.Round(_position.Y);
            return _view;
        }
    }

    public void Add(IRenderable renderable)
    {
        _objectsToFollow.Add(renderable);
    }

    public void Update(GameTime gameTime, Controls controls)
    {
        _position.X += (_objectsToFollow[_currentObject].Destination.Center.X - _offset.X - _view.Center.X)
                       * (_followSpeed * gameTime.DeltaTime());
        _position.Y += (_objectsToFollow[_currentObject].Destination.Center.Y - _offset.Y - _view.Center.Y)
                       * (_followSpeed * gameTime.DeltaTime());

        if (controls.HasFlag(Controls.Left))
        {
            _currentObject--;
            if (_currentObject < 0)
            {
                _currentObject = _objectsToFollow.Count - 1;
            }
        }

        if (controls.HasFlag(Controls.Right))
        {
            _currentObject = (_currentObject + 1) % _objectsToFollow.Count;
        }
    }
}