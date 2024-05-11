using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class SettingsPage : Desktop
{
    public event Action OnBackPressed;

    private readonly Dictionary<string, List<string>> _settingsOptions = new()
    {
        { "Setting1", new List<string> { "Option1", "Option2", "Option3" } },
        { "Setting2", new List<string> { "Option1", "Option2" } },
        // Add more settings and their options here
    };

    public SettingsPage()
    {
        var grid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10
        };

        // Set up two columns: one for setting names, one for dropdowns
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

        foreach (var setting in _settingsOptions)
        {
            // Add a row for each setting
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            // Setting name
            var label = new Label { Text = setting.Key, GridRow = grid.RowsProportions.Count - 1, GridColumn = 0 };
            grid.Widgets.Add(label);

            // Dropdown for setting options
            var comboBox = new ComboBox { GridRow = grid.RowsProportions.Count - 1, GridColumn = 1 };
            foreach (var option in setting.Value)
            {
                comboBox.Items.Add(new ListItem(option));
            }
            grid.Widgets.Add(comboBox);
        }

        // Add a row for the back button
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

        // Back Button
        var backButton = new TextButton { Text = "Back", GridRow = grid.RowsProportions.Count - 1 };
        backButton.Click += (_, _) => OnBackPressed?.Invoke();
        grid.Widgets.Add(backButton);

        Root = grid;
    }
}