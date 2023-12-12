using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MonoGame.Interfaces;
using MonoGame.MenuPages;

namespace MonoGame.Orchestration;

public class PageManager
{
    private readonly IPlayer _player;
    private readonly Dictionary<string, Page> _pages;
    private readonly Stack<string> _currentPageNames;

    public PageManager(IPlayer player, Dictionary<string, Page> pages = null)
    {
        _player = player;
        _currentPageNames = new Stack<string>();
        _pages = pages ?? new Dictionary<string, Page>();
    }
    
    private Page CurrentPage => _currentPageNames.TryPeek(out var currentPageName) 
        ? _pages[currentPageName] 
        : null;

    public PageManager AddPage(string key, Page page)
    {
        _pages[key] = page;
        return this;
    }

    public void SwitchToPage(string pageName)
    {
        if (_pages.ContainsKey(pageName))
        {
            _currentPageNames.Push(pageName);
        }
    }

    public void GoBack()
    {
        if (_currentPageNames.Count > 1)
            _currentPageNames.Pop();
    }

    public void Update()
    {
        var controls = _player.Controls;
        var mouseState = Mouse.GetState();
        var mousePosition = mouseState.Position;
        var isMouseButtonDown = mouseState.LeftButton == ButtonState.Pressed;
        CurrentPage?.Update(controls, mousePosition, isMouseButtonDown);
    }

    public void Draw()
    {
        _player.BeginDisplay();
        CurrentPage?.Draw(_player);
        _player.EndDisplay();
    }
}
