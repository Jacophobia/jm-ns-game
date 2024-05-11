using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Menu.Settings;

[Serializable]
public class GameSettings
{
    // Properties for Main Menu Settings
    public string SelectedStartOption { get; set; }

    // Properties for Start Page Settings
    public string StartPageOption { get; set; }

    // Properties for Controls Page Settings
    [XmlArray("KeyboardMappings")]
    [XmlArrayItem("Mapping")]
    public List<ControlMapping> KeyboardMappings { get; set; }

    [XmlArray("ControllerMappings")]
    [XmlArrayItem("Mapping")]
    public List<ControlMapping> ControllerMappings { get; set; }

    // Properties for Settings Page
    [XmlArray("SettingsOptions")]
    [XmlArrayItem("Option")]
    public Dictionary<string, string> SettingsOptions { get; set; }

    // Properties for Multiplayer Page
    public string MultiplayerOption { get; set; }

    // Properties for Character Selection
    public string SelectedCharacter { get; set; }

    public GameSettings()
    {
        KeyboardMappings = new List<ControlMapping>();
        ControllerMappings = new List<ControlMapping>();
        SettingsOptions = new Dictionary<string, string>();
    }
}

[Serializable]
public class ControlMapping
{
    public string Control { get; set; }
    public string Mapping { get; set; }

    public ControlMapping() { }

    public ControlMapping(string control, string mapping)
    {
        Control = control;
        Mapping = mapping;
    }
}