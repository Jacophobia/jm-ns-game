using System;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class StartPage : Desktop
{
    public event Action OnSinglePlayerClicked;
    public event Action OnMultiplayerClicked;
    public event Action OnBackPressed;

    public StartPage()
    {
        var grid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Set up rows for each button
        for (var i = 0; i < 3; i++)
        {
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        }

        // Single Player Button
        var singlePlayerButton = new TextButton { Text = "Single Player", GridRow = 0 };
        singlePlayerButton.Click += (_, _) => OnSinglePlayerClicked?.Invoke();
        grid.Widgets.Add(singlePlayerButton);

        // Multiplayer Button
        var multiplayerButton = new TextButton { Text = "Multiplayer", GridRow = 1 };
        multiplayerButton.Click += (_, _) => OnMultiplayerClicked?.Invoke();
        grid.Widgets.Add(multiplayerButton);

        // Back Button
        var backButton = new TextButton { Text = "Back", GridRow = 2 };
        backButton.Click += (_, _) => OnBackPressed?.Invoke();
        grid.Widgets.Add(backButton);

        // Set the grid as the root widget
        Root = grid;
    }
}