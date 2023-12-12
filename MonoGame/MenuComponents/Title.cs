﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Interfaces;

namespace MonoGame.MenuComponents;

public class Title : Component
{
    private readonly string _text;
    private readonly SpriteFont _font;

    public Title(Texture2D texture, Rectangle destination, string text, SpriteFont font) 
        : base(texture, destination)
    {
        _text = text;
        _font = font;
    }

    protected override void OnSelect()
    {
        // Titles are non-interactive, so no logic here.
    }

    protected override void OnRender(IPlayer player)
    {
        var textPosition = new Vector2(Destination.X, Destination.Y);
        player.Display(_font, _text, textPosition, Color);
    }
}
