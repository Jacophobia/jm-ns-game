using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public class CharacterSelection : Page
{
    private readonly SpriteFont _font;
    private readonly Texture2D _buttonTexture;

    public CharacterSelection(Rectangle bounds, SpriteFont font, Texture2D buttonTexture, IEnumerable<string> characters) 
        : base(bounds, new List<Component>())
    {
        _font = font;
        _buttonTexture = buttonTexture;

        // Define layout properties
        const int buttonWidth = 200;
        const int buttonHeight = 50;
        const int spacing = 60;
        var startX = Bounds.Center.X;
        var startY = Bounds.Height / 4;

        // Create and add a button for each character
        foreach (var character in characters)
        {
            AddCharacterButton(character, new Rectangle(startX, startY, buttonWidth, buttonHeight));
            startY += spacing;
        }

        // Add "New Character" button
        AddCharacterButton("New Character", new Rectangle(startX, startY, buttonWidth, buttonHeight));
    }

    private void AddCharacterButton(string characterName, Rectangle destination)
    {
        var button = new Button(_buttonTexture, destination, characterName, _font);
        button.SelectEvent += () => HandleCharacterSelect(characterName);
        MenuItems.Add(button);
    }

    private void HandleCharacterSelect(string characterName)
    {
        if (characterName == "New Character")
        {
            // Logic for creating a new character
        }
        else
        {
            // Logic for selecting an existing character
            // e.g., Load character data, proceed to the next game stage, etc.
        }
    }

    protected override void OnDraw(IPlayer player)
    {
        
    }
}
