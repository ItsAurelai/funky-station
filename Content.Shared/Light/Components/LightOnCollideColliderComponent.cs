// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Light.Components;

/// <summary>
/// Can activate <see cref="LightOnCollideComponent"/> when collided with.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class LightOnCollideColliderComponent : Component
{
    [DataField]
    public string FixtureId = "lightTrigger";
}
