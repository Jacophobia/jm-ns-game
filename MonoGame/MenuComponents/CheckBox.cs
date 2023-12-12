using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class CheckBox : Component
{
    private bool _isChecked;
    private readonly SpriteFont _font;
    private readonly string _label;

    public CheckBox(Texture2D texture, Rectangle destination, string label, SpriteFont font, bool isChecked = false) 
        : base(texture, destination)
    {
        _label = label;
        _font = font;
        _isChecked = isChecked;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => _isChecked = value;
    }

    protected override void OnSelect()
    {
        _isChecked = !_isChecked;
    }

    protected override void OnRender(IPlayer player)
    {
        // Render the checkmark if checked
        Color = _isChecked ? Color.DarkGray : Color.White;

        // Render the label next to the checkbox
        var labelPosition = new Vector2(Destination.Right + 10, Destination.Y);
        player.Display(_font, _label, labelPosition, Color);
    }
}
