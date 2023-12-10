using MonoGame.Input;

namespace MonoGame.Interfaces;

public interface IControlSource
{
    public Controls GetControls(IPlayer player);
}