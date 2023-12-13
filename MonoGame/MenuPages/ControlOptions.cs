using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public class ControlOptions : Page, IWritable
{
    public ControlOptions(Rectangle bounds, SpriteFont font, Texture2D controllerTexture) : base(bounds, new List<Component>())
    {
        Text = "Default Controls Text"; // Initialize with default text
        Font = font;

        // Define the position and size for the controller image
        const int imageWidth = 300;
        const int imageHeight = 200;
        var imageX = Bounds.Center.X;
        var imageY = Bounds.Center.Y;

        // Create and add the controller image component
        var controllerImage = new Image(controllerTexture, new Rectangle(imageX, imageY, imageWidth, imageHeight));
        MenuItems.Add(controllerImage);
        
        // TODO: Add a back button to every page except the landing page
    }

    public SpriteFont Font { get; }
    public string Text { get; private set; }

    public Vector2 Position => new (Bounds.Center.X, Bounds.Height / 4f);
    public Color TextColor => Color.White;
    public float Rotation => 0f;
    public Vector2 Origin => Vector2.Zero;
    public Vector2 Scale => Vector2.One * 5;
    public SpriteEffects Effects => SpriteEffects.None;
    public float LayerDepth => -1;

    public void SetDynamicText(string text)
    {
        Text = text;
    }

    protected override void OnDraw(IPlayer player)
    {
        player.Display(this);
    }
}
