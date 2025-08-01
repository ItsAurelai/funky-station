// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.UserInterface.Controls;
using Content.Shared.Silicons.StationAi;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Linq;
using System.Numerics;

namespace Content.Client.Silicons.StationAi;

[GenerateTypedNameReferences]
public sealed partial class StationAiCustomizationMenu : FancyWindow
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    private Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, StationAiCustomizationGroupContainer> _groupContainers = new();
    private Dictionary<ProtoId<StationAiCustomizationGroupPrototype>, ButtonGroup> _buttonGroups = new();

    public event Action<ProtoId<StationAiCustomizationGroupPrototype>, ProtoId<StationAiCustomizationPrototype>>? SendStationAiCustomizationMessageAction;

    private const float IconScale = 1.75f;

    public StationAiCustomizationMenu(EntityUid owner)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        var stationAiSystem = _entManager.System<SharedStationAiSystem>();

        // Load customziation data
        _entManager.TryGetComponent<StationAiCoreComponent>(owner, out var stationAiCore);
        stationAiSystem.TryGetHeld((owner, stationAiCore), out var insertedAi);
        _entManager.TryGetComponent<StationAiCustomizationComponent>(insertedAi, out var stationAiCustomization);

        // Create UI entires for each group of customizations
        var groupPrototypes = _protoManager.EnumeratePrototypes<StationAiCustomizationGroupPrototype>();
        groupPrototypes = groupPrototypes.OrderBy(x => x.ID); // To ensure consistency in presentation

        foreach (var groupPrototype in groupPrototypes)
        {
            StationAiCustomizationPrototype? selectedPrototype = null;

            if (stationAiCustomization?.ProtoIds.TryGetValue(groupPrototype, out var selectedProtoId) == true)
                _protoManager.TryIndex(selectedProtoId, out selectedPrototype);

            _buttonGroups[groupPrototype] = new ButtonGroup();
            _groupContainers[groupPrototype] = new StationAiCustomizationGroupContainer(groupPrototype, selectedPrototype, _buttonGroups[groupPrototype], this, _protoManager);
            CustomizationGroupsContainer.AddTab(_groupContainers[groupPrototype], Loc.GetString(groupPrototype.Name));
        }

        Title = Loc.GetString("station-ai-customization-menu");
    }

    public void OnSendStationAiCustomizationMessage
        (ProtoId<StationAiCustomizationGroupPrototype> groupProtoId, ProtoId<StationAiCustomizationPrototype> customizationProtoId)
    {
        SendStationAiCustomizationMessageAction?.Invoke(groupProtoId, customizationProtoId);
    }

    private sealed class StationAiCustomizationGroupContainer : BoxContainer
    {
        public StationAiCustomizationGroupContainer
            (StationAiCustomizationGroupPrototype groupPrototype,
            StationAiCustomizationPrototype? selectedPrototype,
            ButtonGroup buttonGroup,
            StationAiCustomizationMenu menu,
            IPrototypeManager protoManager)
        {
            Orientation = LayoutOrientation.Vertical;
            HorizontalExpand = true;
            VerticalExpand = true;

            // Create UI entries for all customization in the group
            foreach (var protoId in groupPrototype.ProtoIds)
            {
                if (!protoManager.TryIndex(protoId, out var prototype))
                    continue;

                var entry = new StationAiCustomizationEntryContainer(groupPrototype, prototype, buttonGroup, menu);
                AddChild(entry);

                if (prototype == selectedPrototype)
                    entry.SelectButton.Pressed = true;
            }
        }
    }

    private sealed class StationAiCustomizationEntryContainer : BoxContainer
    {
        public ProtoId<StationAiCustomizationPrototype> ProtoId;
        public Button SelectButton;

        public StationAiCustomizationEntryContainer
            (StationAiCustomizationGroupPrototype groupPrototype,
            StationAiCustomizationPrototype prototype,
            ButtonGroup buttonGroup,
            StationAiCustomizationMenu menu)
        {
            ProtoId = prototype;

            Orientation = LayoutOrientation.Horizontal;
            HorizontalExpand = true;

            // Create a selection button
            SelectButton = new Button
            {
                Text = Loc.GetString(prototype.Name),
                HorizontalExpand = true,
                ToggleMode = true,
                Group = buttonGroup,
            };

            SelectButton.OnPressed += args =>
            {
                menu.OnSendStationAiCustomizationMessage(groupPrototype, prototype);
            };

            AddChild(SelectButton);

            // Creat a background for the preview
            var background = new AnimatedTextureRect
            {
                HorizontalAlignment = HAlignment.Center,
                VerticalAlignment = VAlignment.Center,
                SetWidth = 56,
                SetHeight = 56,
                Margin = new Thickness(10f, 2f),
            };

            background.DisplayRect.TextureScale = new Vector2(IconScale, IconScale);

            if (prototype.PreviewBackground != null)
            {
                background.SetFromSpriteSpecifier(prototype.PreviewBackground);
            }

            AddChild(background);

            // Create a preview icon
            var icon = new AnimatedTextureRect
            {
                HorizontalAlignment = HAlignment.Center,
                VerticalAlignment = VAlignment.Center,
                SetWidth = 56,
                SetHeight = 56,
            };

            icon.DisplayRect.TextureScale = new Vector2(IconScale, IconScale);

            // Default RSI path/state
            var rsiPath = prototype.LayerData.FirstOrNull()?.Value.RsiPath;
            var rsiState = prototype.LayerData.FirstOrNull()?.Value.State;

            // Specified RSI path/state
            if (!string.IsNullOrEmpty(prototype.PreviewKey) && prototype.LayerData.TryGetValue(prototype.PreviewKey, out var layerData))
            {
                rsiPath = layerData.RsiPath;
                rsiState = layerData.State;
            }

            // Update icon
            if (rsiPath != null && rsiState != null)
            {
                var specifier = new SpriteSpecifier.Rsi(new ResPath(rsiPath), rsiState);
                icon.SetFromSpriteSpecifier(specifier);
            }

            background.AddChild(icon);
        }
    }
}


