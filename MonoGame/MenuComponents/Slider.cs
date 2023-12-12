using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Slider : Component
{
    private readonly SpriteFont _font;
    private readonly string _label;
    private readonly int _minValue;
    private readonly int _maxValue;
    private readonly int _currentValue;
    private readonly Texture2D _thumbTexture;
    private Rectangle _thumbRectangle;

    public Slider(Texture2D trackTexture, Texture2D thumbTexture, Rectangle destination, string label, 
        SpriteFont font, int minValue, int maxValue, int initialValue) 
        : base(trackTexture, destination)
    {
        _font = font;
        _label = label;
        _minValue = minValue;
        _maxValue = maxValue;
        _currentValue = initialValue;
        _thumbTexture = thumbTexture;

        // Calculate thumb position based on the initial value
        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        const int thumbWidth = 10; // Example width, adjust as needed
        var thumbHeight = Destination.Height; // Height of the thumb matches the track
        var thumbX = Destination.X + (int)((float)(_currentValue - _minValue) / (_maxValue - _minValue) * Destination.Width) - thumbWidth / 2;
        var thumbY = Destination.Y;

        _thumbRectangle = new Rectangle(thumbX, thumbY, thumbWidth, thumbHeight);
    }

    protected override void OnSelect()
    {
        // Logic for when the slider is selected
        // This might involve changing _currentValue based on input and updating the thumb position
    }

    protected override void OnRender(IPlayer player)
    {
        // Draw the thumb
        player.Display(new Image(_thumbTexture, _thumbRectangle));
        
        // Optionally, draw the label
        var labelPosition = new Vector2(Destination.X, Destination.Y - _font.LineSpacing);
        player.Display(_font, _label, labelPosition, Color.White);
    }
}
