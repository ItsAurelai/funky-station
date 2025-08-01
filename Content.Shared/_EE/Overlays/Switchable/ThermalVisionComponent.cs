// SPDX-FileCopyrightText: 2025 V <97265903+formlessnameless@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared._EE.Overlays.Switchable;

[RegisterComponent, NetworkedComponent]
public sealed partial class ThermalVisionComponent : SwitchableOverlayComponent
{
    public override string? ToggleAction { get; set; } = "ToggleThermalVision";

    public override Color Color { get; set; } = Color.FromHex("#F84742");

    [DataField]
    public float LightRadius = 5f;
}

public sealed partial class ToggleThermalVisionEvent : InstantActionEvent;
