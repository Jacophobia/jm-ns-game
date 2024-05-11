using System;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class MultiplayerPage : Desktop
{
    public event Action OnStartGameClicked;
    public event Action OnJoinGameClicked;
    public event Action OnBackPressed;

    public MultiplayerPage()
    {
        var grid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Set up rows for each button and the back button
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

        // Start Game Button
        var startGameButton = new TextButton { Text = "Start a Game", GridRow = 0 };
        startGameButton.Click += (_, _) => OnStartGameClicked?.Invoke();
        grid.Widgets.Add(startGameButton);

        // Join Game Button
        var joinGameButton = new TextButton { Text = "Join a Game", GridRow = 1 };
        joinGameButton.Click += (_, _) => OnJoinGameClicked?.Invoke();
        grid.Widgets.Add(joinGameButton);

        // Back Button
        var backButton = new TextButton { Text = "Back", GridRow = 2 };
        backButton.Click += (_, _) => OnBackPressed?.Invoke();
        grid.Widgets.Add(backButton);

        // Set the grid as the root widget
        Root = grid;
    }
}