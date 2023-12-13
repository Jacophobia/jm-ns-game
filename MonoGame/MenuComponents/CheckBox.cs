using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class CheckBox : Component, IWritable
{
    private bool _isChecked;

    public CheckBox(Texture2D texture, Rectangle destination, string label, SpriteFont font, bool isChecked = false) 
        : base(texture, destination)
    {
        Text = label;
        Font = font;
        
        _isChecked = isChecked;
    }
    
    public SpriteFont Font { get; }
    public string Text { get; }
    public Vector2 Position => new (Destination.Right + 10, Destination.Y);
    public Color TextColor => Color.Black;
    public Vector2 Scale => Vector2.One * 5;
    public SpriteEffects Effects => SpriteEffects.None;
    public float LayerDepth => Depth - 1;

    protected override void OnSelect()
    {
        _isChecked = !_isChecked;
    }

    protected override void OnRender(IPlayer player)
    {
        // Render the checkmark if checked
        Color = _isChecked ? Color.DarkGray : Color.LightGray;
        player.Display(this);
    }

}
