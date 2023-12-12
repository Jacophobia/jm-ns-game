using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public class SettingsPage : Page
{
    private readonly SpriteFont _font;
    private readonly Texture2D _checkboxTexture;
    private readonly Texture2D _sliderTexture; // Texture for slider background
    private readonly Texture2D _sliderThumbTexture; // Texture for slider thumb

    public SettingsPage(SpriteFont font, Texture2D checkboxTexture, Texture2D sliderTexture, Texture2D sliderThumbTexture) 
        : base(new List<Component>())
    {
        _font = font;
        _checkboxTexture = checkboxTexture;
        _sliderTexture = sliderTexture;
        _sliderThumbTexture = sliderThumbTexture;

        // Example positions and sizes (customize as needed)
        const int startX = 100;
        const int startY = 100;
        const int spacing = 50;
        const int componentWidth = 200;
        const int componentHeight = 40;

        // Volume Slider
        AddSlider("Volume", new Rectangle(startX, startY, componentWidth, componentHeight));

        // V-sync Checkbox
        AddCheckbox("V-sync", new Rectangle(startX, startY + spacing, componentWidth, componentHeight));

        // Fullscreen Checkbox
        AddCheckbox("Fullscreen", new Rectangle(startX, startY + 2 * spacing, componentWidth, componentHeight));

        // Other settings can be added similarly
    }

    private void AddCheckbox(string label, Rectangle destination)
    {
        var checkbox = new CheckBox(_checkboxTexture, destination, label, _font);
        // Configure checkbox event and initial state
        MenuItems.Add(checkbox);
    }

    private void AddSlider(string label, Rectangle destination)
    {
        var slider = new Slider(_sliderTexture, _sliderThumbTexture, destination, label, _font, 0, 100, 50); // Example range: 0-100, initial value: 50
        // Configure slider events and properties
        MenuItems.Add(slider);
    }

    // Implement additional methods or components as necessary
    protected override void OnDraw(IPlayer player)
    {
        
    }
}
