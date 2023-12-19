using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Singletons;

// ReSharper disable once InvertIf
internal class FontManager
{
    private static FontManager _instance = null;

    private readonly IDictionary<string, SpriteFont> _fonts;
    private readonly ContentManager _contentManager;

    internal SpriteFont this[string key]
    {
        get
        {
            if (!_fonts.TryGetValue(key, out var font))
            {
                font = _contentManager.Load<SpriteFont>(key);
                _fonts.TryAdd(key, font);
            }
            
            return font;
        }
    }

    private FontManager(ContentManager contentManager)
    {
        _contentManager = contentManager;
        _fonts = new Dictionary<string, SpriteFont>();
    }

    internal static void Initialize(ContentManager contentManager)
    {
        _instance ??= new FontManager(contentManager);
    }

    internal static FontManager GetInstance()
    {
        return _instance ?? throw new Exception("The font manager has not yet been initialized. Please initialize it by calling TextureManager.Initialize(ContentManager).");
    }
}