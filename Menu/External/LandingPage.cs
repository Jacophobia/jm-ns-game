using System;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class LandingPage : Desktop
{
    public event Action OnStartClicked;
    public event Action OnControlsClicked;
    public event Action OnSettingsClicked;
    public event Action OnExitClicked;

    public LandingPage()
    {
        var grid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Set up rows for each button
        for (var i = 0; i < 4; i++)
        {
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        }

        // Start Button
        var startButton = new TextButton { Text = "Start", GridRow = 0 };
        startButton.Click += (_, _) => OnStartClicked?.Invoke();
        grid.Widgets.Add(startButton);

        // Controls Button
        var controlsButton = new TextButton { Text = "Controls", GridRow = 1 };
        controlsButton.Click += (_, _) => OnControlsClicked?.Invoke();
        grid.Widgets.Add(controlsButton);

        // Settings Button
        var settingsButton = new TextButton { Text = "Settings", GridRow = 2 };
        settingsButton.Click += (_, _) => OnSettingsClicked?.Invoke();
        grid.Widgets.Add(settingsButton);

        // Exit Button
        var exitButton = new TextButton { Text = "Exit", GridRow = 3 };
        exitButton.Click += (_, _) => OnExitClicked?.Invoke();
        grid.Widgets.Add(exitButton);

        // Set the grid as the root widget
        Root = grid;
    }
}