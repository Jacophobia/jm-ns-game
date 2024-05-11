using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class ControlsPage : Desktop
{
    public event Action OnBackPressed;

    private readonly List<string> _controlOptions = new()
    {
        "None", "Up", "Down", "Left", "Right", "Jump",
        "ModifierOne", "ModifierTwo", "ModifierThree", "ModifierFour",
        "CommandOne", "CommandTwo", "CommandThree", "CommandFour",
        "Start", "Select"
    };

    public ControlsPage(ContentManager content)
    {
        var grid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8
        };

        // Load the controller image
        var controllerTexture = content.Load<Texture2D>("Content/Menu/Controller");
        var controllerImage = new Image
        {
            Texture = controllerTexture,
            GridRow = 0,
            GridColumnSpan = 3
        };
        grid.Widgets.Add(controllerImage);

        // Set up rows and columns for control options and the back button
        for (var i = 0; i < _controlOptions.Count + 1; i++) // +1 for the back button
        {
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        }
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // For labels
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // For keyboard dropdowns
        grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto)); // For controller dropdowns

        // Adding control options
        var rowIndex = 1; // Start from 1 to leave space for the controller image
        foreach (var option in _controlOptions)
        {
            var label = new Label { Text = option, GridRow = rowIndex, GridColumn = 0 };
            var keyboardDropDown = new ComboBox { GridRow = rowIndex, GridColumn = 1 };
            var controllerDropDown = new ComboBox { GridRow = rowIndex, GridColumn = 2 };

            foreach (var controlOption in _controlOptions)
            {
                keyboardDropDown.Items.Add(new ListItem(controlOption));
                controllerDropDown.Items.Add(new ListItem(controlOption));
            }

            grid.Widgets.Add(label);
            grid.Widgets.Add(keyboardDropDown);
            grid.Widgets.Add(controllerDropDown);
            rowIndex++;
        }

        // Back Button
        var backButton = new TextButton { Text = "Back", GridRow = rowIndex };
        backButton.Click += (_, _) => OnBackPressed?.Invoke();
        grid.Widgets.Add(backButton);

        Root = grid;
    }
}