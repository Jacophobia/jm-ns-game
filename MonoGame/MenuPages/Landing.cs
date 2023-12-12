using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public class Landing : Page
{
    private readonly SpriteFont _font; // Assuming you have a SpriteFont for text
    private readonly Texture2D _buttonTexture; // Assuming you have a Texture2D for buttons

    public Landing(SpriteFont font, Texture2D buttonTexture) : base(new List<Component>())
    {
        _font = font;
        _buttonTexture = buttonTexture;

        // Define button sizes and positions
        const int buttonWidth = 200;
        const int buttonHeight = 50;
        const int spacing = 60;
        var startX = /* Calculate X position */;
        var startY = /* Calculate Y position */;

        // Create and add buttons
        AddButton("Start", new Rectangle(startX, startY, buttonWidth, buttonHeight));
        AddButton("Controls", new Rectangle(startX, startY + spacing, buttonWidth, buttonHeight));
        AddButton("Settings", new Rectangle(startX, startY + 2 * spacing, buttonWidth, buttonHeight));
        AddButton("Exit", new Rectangle(startX, startY + 3 * spacing, buttonWidth, buttonHeight));
    }

    private void AddButton(string text, Rectangle destination)
    {
        var button = new Button(_buttonTexture, destination, text, _font);
        button.SelectEvent += () => HandleButtonSelect(text);
        MenuItems.Add(button);
    }

    private void HandleButtonSelect(string buttonText)
    {
        switch (buttonText)
        {
            case "Start":
                // Logic for Start button
                break;
            case "Controls":
                // Logic for Controls button
                break;
            case "Settings":
                // Logic for Settings button
                break;
            case "Exit":
                // Logic for Exit button
                break;
        }
    }
}
