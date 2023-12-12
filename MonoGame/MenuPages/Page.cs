using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.MenuComponents;

namespace MonoGame.MenuPages;

public abstract class Page
{
    protected readonly List<Component> MenuItems;
    private int _selectedMenuIndex;

    protected Page(List<Component> menuItems)
    {
        MenuItems = menuItems;
    }

    public void Update(Controls controls, Point mousePosition, bool isMouseButtonDown)
    {
        var selected = false;
        
        if ((controls & Controls.Up) != 0)
        {
            _selectedMenuIndex--;

            if (_selectedMenuIndex < 0)
            {
                _selectedMenuIndex = MenuItems.Count - _selectedMenuIndex;
            }
        }
        if ((controls & Controls.Down) != 0)
        {
            _selectedMenuIndex = (_selectedMenuIndex + 1) % MenuItems.Count;
        }
        if ((controls & Controls.Jump) != 0)
        {
            selected = true;
        }

        for (var i = 0; i < MenuItems.Count; i++)
        {
            MenuItems[i].Update(selected && i == _selectedMenuIndex, mousePosition, isMouseButtonDown);
        }
    }

    public void Draw(IPlayer player)
    {
        OnDraw(player);
        
        for (var i = 0; i < MenuItems.Count; i++)
        {
            MenuItems[i].Color = i == _selectedMenuIndex 
                ? Color.Gray 
                : Color.White;
            
            MenuItems[i].Render(player);
        }
    }

    protected abstract void OnDraw(IPlayer player);
}
