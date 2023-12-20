using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.MenuPages;

namespace MonoGame.Orchestration;

public class MenuFactory
{
    private readonly Rectangle _bounds;
    private readonly SpriteFont _font;
    private readonly Texture2D _buttonTexture;
    private readonly Texture2D _checkboxTexture;
    private readonly Texture2D _sliderTexture;
    private readonly Texture2D _sliderThumbTexture;
    private readonly Texture2D _controllerTexture;

    public MenuFactory(Rectangle menuBounds, SpriteFont font, Texture2D buttonTexture, Texture2D checkboxTexture, 
                       Texture2D sliderTexture, Texture2D sliderThumbTexture, Texture2D controllerTexture)
    {
        _bounds = menuBounds;
        _font = font;
        _buttonTexture = buttonTexture;
        _checkboxTexture = checkboxTexture;
        _sliderTexture = sliderTexture;
        _sliderThumbTexture = sliderThumbTexture;
        _controllerTexture = controllerTexture;
    }

    public PageManager CreateMenu(IPlayer player, IEnumerable<string> characters)
    {
        var pageManager = new PageManager(player);

        // Create and setup each page with necessary components
        var landingPage = CreateLandingPage();
        var startPage = CreateStartPage();
        var controlsPage = CreateControlsPage();
        var settingsPage = CreateSettingsPage();
        var multiplayerPage = CreateMultiplayerPage();
        var characterSelectionPage = CreateCharacterSelectionPage(characters);

        // Add pages to the PageManager
        pageManager
            .AddPage("Landing", landingPage)
            .AddPage("Start", startPage)
            .AddPage("Controls", controlsPage)
            .AddPage("Settings", settingsPage)
            .AddPage("Multiplayer", multiplayerPage)
            .AddPage("CharacterSelection", characterSelectionPage);

        return pageManager;
    }

    private Landing CreateLandingPage()
    {
        return new Landing(_bounds, _font, _buttonTexture);
    }

    private Start CreateStartPage()
    {
        return new Start(_bounds, _font, _buttonTexture);
    }

    private ControlOptions CreateControlsPage()
    {
        // Replace with the actual controller texture
        return new ControlOptions(_bounds, _font, _controllerTexture);
    }

    private SettingsPage CreateSettingsPage()
    {
        return new SettingsPage(_bounds, _font, _checkboxTexture, _sliderTexture, _sliderThumbTexture);
    }

    private MultiplayerSelect CreateMultiplayerPage()
    {
        return new MultiplayerSelect(_bounds, _font, _buttonTexture);
    }

    private CharacterSelection CreateCharacterSelectionPage(IEnumerable<string> characters)
    {
        return new CharacterSelection(_bounds, _font, _buttonTexture, characters);
    }
}
