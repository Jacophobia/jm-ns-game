using Shared.Players;

namespace Shared.Input;

public interface IControlSource
{
    public Controls GetControls(IPlayer player);
}