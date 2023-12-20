using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Title : Component, IWritable
{
    public Title(Texture2D texture, Rectangle destination, string text, SpriteFont font) 
        : base(texture, destination)
    {
        Text = text;
        Font = font;
    }

    public SpriteFont Font { get; }
    public string Text { get; }
    public Vector2 Position => new (Destination.X, Destination.Y);
    public Color TextColor => Color.White;
    public Vector2 Scale => Vector2.One * 5;
    public SpriteEffects Effects => SpriteEffects.None;
    public float LayerDepth => Depth - 1;

    protected override void OnSelect()
    {
        // Titles are non-interactive, so no logic here.
    }

    protected override void OnRender(IPlayer player)
    {
        player.Display(this as IWritable);
    }
}
