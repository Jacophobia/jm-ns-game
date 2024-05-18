using Shared.Controllers;

namespace Combat.Fighters;

public class Fighter
{
    private readonly IController _controller;
    
    // TODO: The fighter needs to take in a ControlSource as a parameter
    // The control source could be either a player control source or the
    //  ai that we use to control the other fighter
    public Fighter(IController controller)
    {
        throw new System.NotImplementedException();
    }

    public void Update(float deltaTime)
    {
        var controls = _controller.Controls;
        throw new System.NotImplementedException();
    }

    public void Draw()
    {
        throw new System.NotImplementedException();
    }
}