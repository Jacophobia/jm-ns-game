using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public class Start : Page
{
    private readonly SpriteFont _font; // Assuming you have a SpriteFont for text
    private readonly Texture2D _buttonTexture; // Assuming you have a Texture2D for buttons

    public Start(Rectangle bounds, SpriteFont font, Texture2D buttonTexture) : base(bounds, new List<Component>())
    {
        _font = font;
        _buttonTexture = buttonTexture;

        // Define button sizes and positions
        const int buttonWidth = 200;
        const int buttonHeight = 50;
        const int spacing = 60;
        var startX = Bounds.Center.X;
        var startY = Bounds.Height / 4;

        // Create and add buttons
        AddButton("Single Player", new Rectangle(startX, startY, buttonWidth, buttonHeight));
        AddButton("Multiplayer", new Rectangle(startX, startY + spacing, buttonWidth, buttonHeight));
        AddButton("Back", new Rectangle(startX, startY + 2 * spacing, buttonWidth, buttonHeight));
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
            case "Single Player":
                // Logic for Single Player button
                break;
            case "Multiplayer":
                // Logic for Multiplayer button
                break;
            case "Back":
                // Logic for Back button
                break;
        }
    }

    protected override void OnDraw(IPlayer player)
    {
        
    }
}
