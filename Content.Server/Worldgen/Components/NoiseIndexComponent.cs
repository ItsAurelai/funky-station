// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Moony <moony@hellomouse.net>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Worldgen.Prototypes;
using Content.Server.Worldgen.Systems;

namespace Content.Server.Worldgen.Components;

/// <summary>
///     This is used for containing configured noise generators.
/// </summary>
[RegisterComponent]
[Access(typeof(NoiseIndexSystem))]
public sealed partial class NoiseIndexComponent : Component
{
    /// <summary>
    ///     An index of generators, to avoid having to recreate them every time a noise channel is used.
    ///     Keyed by noise generator prototype ID.
    /// </summary>
    [Access(typeof(NoiseIndexSystem), Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.None)]
    public Dictionary<string, NoiseGenerator> Generators { get; } = new();
}

