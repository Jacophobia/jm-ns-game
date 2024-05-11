using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI;

namespace Menu.External;

public class CharacterSelectionPage : Desktop
{
    public event Action<string> OnCharacterSelected;
    public event Action OnNewCharacterClicked;
    public event Action OnBackPressed;

    public CharacterSelectionPage(List<string> availableCharacters)
    {
        var grid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Add a row for each character, one for 'New Character', and one for 'Back' button
        foreach (var _ in availableCharacters)
        {
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
        }
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // For 'New Character' button
        grid.RowsProportions.Add(new Proportion(ProportionType.Auto)); // For 'Back' button

        // Add character selection buttons
        var rowIndex = 0;
        foreach (var character in availableCharacters)
        {
            var characterButton = new TextButton { Text = character, GridRow = rowIndex };
            characterButton.Click += (_, _) => OnCharacterSelected?.Invoke(character);
            grid.Widgets.Add(characterButton);
            rowIndex++;
        }

        // New Character button
        var newCharacterButton = new TextButton { Text = "New Character", GridRow = rowIndex };
        newCharacterButton.Click += (_, _) => OnNewCharacterClicked?.Invoke();
        grid.Widgets.Add(newCharacterButton);

        // Back Button
        var backButton = new TextButton { Text = "Back", GridRow = ++rowIndex };
        backButton.Click += (_, _) => OnBackPressed?.Invoke();
        grid.Widgets.Add(backButton);

        Root = grid;
    }
}