// SPDX-FileCopyrightText: 2025 corresp0nd <46357632+corresp0nd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Funkystation.Medical.Records.UI;

/// <summary>
/// For generating a new checklist entry for allergies, prescriptions, family history
/// </summary>
[GenerateTypedNameReferences]
public sealed partial class RecordChecklistEntry : BoxContainer
{
    public event Action<bool>? PreferenceChanged;

    private readonly CheckBox _checkbox;
    private readonly Label _label;

    public bool Preference
    {
        get => ItemCheckBox.Pressed;
        set => ItemCheckBox.Pressed = value;
    }
    public RecordChecklistEntry(string name)
    {
        RobustXamlLoader.Load(this);

        _checkbox = ItemCheckBox;
        _label = ItemLabel;
        _label.Text = name;
        _checkbox.OnToggled += OnCheckBoxToggled;
    }

    private void OnCheckBoxToggled(BaseButton.ButtonToggledEventArgs args)
    {
        PreferenceChanged?.Invoke(args.Pressed);
    }
}
