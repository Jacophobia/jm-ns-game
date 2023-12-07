using MonoGame.Input;

namespace MonoGame.Interfaces;

public interface IUpdatable
{
    public void Update(float deltaTime, Controls controls = Controls.None);
}