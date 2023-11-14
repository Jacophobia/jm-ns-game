using IO.Extensions;
using IO.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IO.Output;

public class Camera
{
    public Rectangle View
    {
        get
        {
            _view.X = (int)_position.X;
            _view.Y = (int)_position.Y;
            return _view;
        }
    }

    private readonly IRenderable _objectToFollow;
    private readonly Vector3 _offset;
    private readonly float _followSpeed;
    private Vector2 _position;
    private Rectangle _view;

    public Camera(IRenderable objectToFollow, float followSpeed, Vector3 offset)
    {
        using var adapter = GraphicsAdapter.DefaultAdapter;
        var displayMode = adapter.CurrentDisplayMode;
        var position = offset + objectToFollow.Destination.Center.ToVector3();
        
        _view = new Rectangle(0, 0, displayMode.Width, displayMode.Height);
        _position = new Vector2(position.X + displayMode.Width / 2f, position.Y + displayMode.Height / 2f);
        _objectToFollow = objectToFollow;
        _followSpeed = followSpeed;
        _offset = offset;
    }

    public void Update(GameTime gameTime)
    {
        _position.X += (_objectToFollow.Destination.Center.X - _offset.X - _view.Center.X) 
                       * (_followSpeed * gameTime.DeltaTime());
        _position.Y += (_objectToFollow.Destination.Center.Y - _offset.Y - _view.Center.Y) 
                       * (_followSpeed * gameTime.DeltaTime());
    }
}