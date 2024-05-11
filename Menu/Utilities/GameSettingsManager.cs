using System.IO;
using System.Xml.Serialization;
using Menu.Settings;

namespace Menu.Utilities;

public static class GameSettingsManager
{
    public static void SaveToXml(GameSettings settings, string filePath)
    {
        var serializer = new XmlSerializer(typeof(GameSettings));
        using (var writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, settings);
        }
    }

    public static GameSettings LoadFromXml(string filePath)
    {
        var serializer = new XmlSerializer(typeof(GameSettings));
        using (var reader = new StreamReader(filePath))
        {
            return (GameSettings)serializer.Deserialize(reader);
        }
    }
}