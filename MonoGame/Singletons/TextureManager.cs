using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Singletons;

// ReSharper disable once InvertIf
internal class TextureManager
{
    private static TextureManager _instance = null;

    private readonly IDictionary<string, Texture2D> _textures;
    private readonly ContentManager _contentManager;

    internal Texture2D this[string key]
    {
        get
        {
            if (!_textures.TryGetValue(key, out var texture))
            {
                texture = _contentManager.Load<Texture2D>(key);
                _textures.Add(key, texture);
            }
            
            return texture;
        }
    }

    private TextureManager(ContentManager contentManager)
    {
        _contentManager = contentManager;
        _textures = new Dictionary<string, Texture2D>();
    }

    internal static void Initialize(ContentManager contentManager)
    {
        _instance ??= new TextureManager(contentManager);
    }

    internal static TextureManager GetInstance()
    {
        return _instance ?? throw new Exception("The texture manager has not yet been initialized. Please initialize it by calling TextureManager.Initialize(ContentManager).");
    }
}