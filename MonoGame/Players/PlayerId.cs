using System.Net;

namespace MonoGame.Players;

public class PlayerId
{
    public PlayerId(object key)
    {
        Key = key;
    }
    
    public object Key { get; } // TODO: not a fan of the use of object here. Should definitely think of a better way of doing this
    public string Id => Key.ToString();
}