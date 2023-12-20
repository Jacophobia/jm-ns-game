using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Image : Component
{
    public Image(Texture2D texture, Rectangle destination) 
        : base(texture, destination)
    {
        // Initialization, if needed
    }

    protected override void OnSelect()
    {
        // Image is non-interactive, so no logic here
    }

    protected override void OnRender(IPlayer player)
    {
        // Simply draw the image
        player.Display(this);
    }
}
