using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Button : Component
{
    private string Text { get; }
    private readonly SpriteFont _font;

    public Button(Texture2D texture, Rectangle destination, string text, SpriteFont font) 
        : base(texture, destination)
    {
        Text = text;
        _font = font;
    }

    protected override void OnSelect()
    {
        // Logic to execute when the button is selected
    }

    protected override void OnRender(IPlayer player)
    {
        var textSize = _font.MeasureString(Text);
        var textPosition = new Vector2(Destination.Center.X - textSize.X / 2, Destination.Center.Y - textSize.Y / 2);
        player.Display(_font, Text, textPosition, Color);
    }
}
