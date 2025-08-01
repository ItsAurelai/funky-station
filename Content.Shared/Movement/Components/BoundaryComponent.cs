// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Represents a boundary that can bump someone back when touched.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BoundaryComponent : Component
{
    [DataField, AutoNetworkedField]
    public float Offset = 2f;
}
