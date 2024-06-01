using Microsoft.Xna.Framework;

namespace Shared.Controllables;

public class DoNothingController : IController
{
    public Controls Controls => Controls.None;
    public Vector2 LeftJoystick => Vector2.Zero;
    public Vector2 RightJoystick => Vector2.Zero;
}