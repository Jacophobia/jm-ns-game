using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Button : Component, IWritable
{
    public Button(Texture2D texture, Rectangle destination, string text, SpriteFont font) 
        : base(texture, destination)
    {
        Text = text;
        Font = font;
    }
    
    public SpriteFont Font { get; }
    public string Text { get; }
    public Vector2 Position
    {
        get
        {
            var textSize = Font.MeasureString(Text);
            return new Vector2(Destination.Center.X - textSize.X / 2, Destination.Center.Y - textSize.Y / 2);
        }
    }

    public Color TextColor => Color.White;
    public Vector2 Scale => Vector2.One * 5;
    public SpriteEffects Effects => SpriteEffects.None;
    public float LayerDepth => Depth - 1;

    protected override void OnSelect()
    {
        // Logic to execute when the button is selected
    }

    protected override void OnRender(IPlayer player)
    {
        player.Display(this as IWritable);
    }
}
