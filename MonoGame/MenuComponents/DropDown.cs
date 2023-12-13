using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class DropDown : Component, IWritable
{
    private readonly List<string> _options;
    private readonly int _selectedIndex;

    public DropDown(Texture2D texture, Rectangle destination, List<string> options, SpriteFont font) 
        : base(texture, destination)
    {
        Font = font;
        
        _options = options;
        _selectedIndex = 0;
    }

    public SpriteFont Font { get; }
    public string Text => _options[_selectedIndex];
    public Vector2 Position => new (Destination.X, Destination.Y);
    public Color TextColor => Color.White;
    public Vector2 Scale => Vector2.One * 5;
    public SpriteEffects Effects => SpriteEffects.None;
    public float LayerDepth => Depth - 1;

    protected override void OnSelect()
    {
        // Logic for cycling through options or opening the dropdown
    }

    protected override void OnRender(IPlayer player)
    {
        player.Display(this);
    }
}
