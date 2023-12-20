﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Input;

namespace MonoGame.Interfaces;

public interface IPlayer
{
    public Guid Id { get; }
    public Rectangle Perspective { get; }
    public float Depth { get; }
    public float FocalLength { get; }
    public Controls Controls { get; }

    public void Follow(IRenderable target);

    public void BeginDisplay();

    public void Display(IRenderable renderable, Texture2D texture = null, Rectangle? destination = null,
        Rectangle? source = null, Color? color = null, float? rotation = null, Vector2? origin = null,
        SpriteEffects effect = SpriteEffects.None, float? depth = null);

    public void Display(IWritable writable, SpriteFont font = null, 
    string text = null, Vector2? position = null, Color? color = null, 
    float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, 
    float? depth = null);
    
    public void Update(float deltaTime);

    public void EndDisplay();
}