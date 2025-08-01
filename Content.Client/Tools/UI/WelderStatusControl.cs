// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2023 Darkie <darksaiyanis@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Items.UI;
using Content.Client.Message;
using Content.Client.Stylesheets;
using Content.Shared.FixedPoint;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Tools.UI;

public sealed class WelderStatusControl : PollingItemStatusControl<WelderStatusControl.Data>
{
    private readonly Entity<WelderComponent> _parent;
    private readonly IEntityManager _entityManager;
    private readonly SharedToolSystem _toolSystem;
    private readonly RichTextLabel _label;

    public WelderStatusControl(Entity<WelderComponent> parent, IEntityManager entityManager, SharedToolSystem toolSystem)
    {
        _parent = parent;
        _entityManager = entityManager;
        _toolSystem = toolSystem;
        _label = new RichTextLabel { StyleClasses = { StyleNano.StyleClassItemStatus } };
        AddChild(_label);

        UpdateDraw();
    }

    protected override Data PollData()
    {
        var (fuel, capacity) = _toolSystem.GetWelderFuelAndCapacity(_parent, _parent.Comp);
        return new Data(fuel, capacity, _parent.Comp.Enabled);
    }

    protected override void Update(in Data data)
    {
        _label.SetMarkup(Loc.GetString("welder-component-on-examine-detailed-message",
            ("colorName", data.Fuel < data.FuelCapacity / 4f ? "darkorange" : "orange"),
            ("fuelLeft", data.Fuel),
            ("fuelCapacity", data.FuelCapacity),
            ("status", Loc.GetString(data.Lit ? "welder-component-on-examine-welder-lit-message" : "welder-component-on-examine-welder-not-lit-message"))));
    }

    public record struct Data(FixedPoint2 Fuel, FixedPoint2 FuelCapacity, bool Lit);
}
