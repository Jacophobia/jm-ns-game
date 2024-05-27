using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Shared.Controllables;

namespace Shared.Configuration;

public class GameSettings
{
    public bool Fullscreen { get; set; }
    public bool IsHost { get; set; }
    public string HostIp { get; set; } 
    public int HostPort { get; set; }
    public Dictionary<Keys, Controls> Controls { get; set; }
}