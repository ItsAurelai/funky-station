// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Temperature.Systems;

namespace Content.Shared.Temperature.Components;

/// <summary>
/// Makes the entity always set <c>IsHotEvent.IsHot</c> to true, no matter what.
/// </summary>
[RegisterComponent, Access(typeof(AlwaysHotSystem))]
public sealed partial class AlwaysHotComponent : Component
{
}
