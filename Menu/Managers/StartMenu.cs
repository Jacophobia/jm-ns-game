using System.Collections.Generic;
using Menu.External;
using Menu.Settings;
using Menu.Utilities;
using Myra.Graphics2D.UI;

namespace Menu.Managers;

public class StartMenu
{
    private readonly Panel _mainContainer;

    private LandingPage _landingPage;
    private StartPage _startPage;
    private ControlsPage _controlsPage;
    private SettingsPage _settingsPage;
    private MultiplayerPage _multiplayerPage;
    private CharacterSelectionPage _characterSelectionPage;

    private readonly GameSettings _gameSettings;

    public StartMenu(Desktop desktop)
    {
        _mainContainer = new Panel();
        _gameSettings = new GameSettings();

        InitializePages();

        ShowPage(_landingPage);

        desktop.Root = _mainContainer;
    }

    private void InitializePages()
    {
        // Initialize pages
        _landingPage = new LandingPage();
        _startPage = new StartPage();
        _controlsPage = new ControlsPage();
        _settingsPage = new SettingsPage();
        _multiplayerPage = new MultiplayerPage();
        _characterSelectionPage = new CharacterSelectionPage(); // Pass character list

        // Subscribe to events
        _landingPage.OnStartClicked += () => ShowPage(_startPage);
        _startPage.OnSinglePlayerClicked += () => ShowPage(_controlsPage);
        _startPage.OnMultiplayerClicked += () => ShowPage(_multiplayerPage);
        _startPage.OnBackPressed += () => ShowPage(_landingPage);
        _controlsPage.OnBackPressed += () => ShowPage(_startPage);
        _settingsPage.OnBackPressed += () => ShowPage(_landingPage);
        _multiplayerPage.OnBackPressed += () => ShowPage(_startPage);
        _characterSelectionPage.OnBackPressed += () => ShowPage(_landingPage);
        _characterSelectionPage.OnCharacterSelected += CharacterSelected;

        // Additional subscriptions for updating settings
        // Assuming these pages provide methods or properties to get the selected settings
        _controlsPage.OnMappingsUpdated += UpdateControlMappings;
        _settingsPage.OnSettingsChanged += UpdateSettingsOptions;
        _multiplayerPage.OnOptionSelected += UpdateMultiplayerOption;
    }

    private void ShowPage(Desktop page)
    {
        _mainContainer.Widgets.Clear();
        _mainContainer.Widgets.Add(page.Root);
    }

    private void UpdateControlMappings(List<ControlMapping> keyboardMappings, List<ControlMapping> controllerMappings)
    {
        _gameSettings.KeyboardMappings = keyboardMappings;
        _gameSettings.ControllerMappings = controllerMappings;
    }

    private void UpdateSettingsOptions(Dictionary<string, string> settingsOptions)
    {
        _gameSettings.SettingsOptions = settingsOptions;
    }

    private void UpdateMultiplayerOption(string option)
    {
        _gameSettings.MultiplayerOption = option;
    }

    private void CharacterSelected(string characterName)
    {
        _gameSettings.SelectedCharacter = characterName;
        // Serialize and send data to the server
        GameSettingsManager.SaveToXml(_gameSettings, "path_to_save_settings.xml");
        // SendDataToServer(gameSettings); // Implement this method as needed
    }
}