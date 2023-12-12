using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class DropDown : Component
{
    private readonly List<string> _options;
    private readonly int _selectedIndex;
    private readonly SpriteFont _font;

    public DropDown(Texture2D texture, Rectangle destination, List<string> options, SpriteFont font) 
        : base(texture, destination)
    {
        _options = options;
        _font = font;
        _selectedIndex = 0;
    }

    protected override void OnSelect()
    {
        // Logic for cycling through options or opening the dropdown
    }

    protected override void OnRender(IPlayer player)
    {
        var selectedOption = _options[_selectedIndex];
        var textPosition = new Vector2(Destination.X, Destination.Y);
        player.Display(_font, selectedOption, textPosition, Color);
    }
}
