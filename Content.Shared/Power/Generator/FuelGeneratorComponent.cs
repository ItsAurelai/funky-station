// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Guidebook;
using Robust.Shared.GameStates;

namespace Content.Shared.Power.Generator;

/// <summary>
/// This is used for generators that run off some kind of fuel.
/// </summary>
/// <remarks>
/// <para>
/// Generators must be anchored to be able to run.
/// </para>
/// </remarks>
/// <seealso cref="SharedGeneratorSystem"/>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SharedGeneratorSystem))]
public sealed partial class FuelGeneratorComponent : Component
{
    /// <summary>
    /// Is the generator currently running?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool On;

    /// <summary>
    /// The generator's target power.
    /// </summary>
    [DataField]
    public float TargetPower = 15_000.0f;

    /// <summary>
    /// The maximum target power.
    /// </summary>
    [DataField]
    [GuidebookData]
    public float MaxTargetPower = 30_000.0f;

    /// <summary>
    /// The minimum target power.
    /// </summary>
    /// <remarks>
    /// Setting this to any value above 0 means that the generator can't idle without consuming some amount of fuel.
    /// </remarks>
    [DataField]
    public float MinTargetPower = 1_000;

    /// <summary>
    /// The "optimal" power at which the generator is considered to be at 100% efficiency.
    /// </summary>
    [DataField]
    public float OptimalPower = 15_000.0f;

    /// <summary>
    /// The rate at which one unit of fuel should be consumed.
    /// </summary>
    [DataField]
    public float OptimalBurnRate = 1 / 60.0f; // Once every 60 seconds.

    /// <summary>
    /// A constant used to calculate fuel efficiency in relation to target power output and optimal power output
    /// </summary>
    [DataField]
    public float FuelEfficiencyConstant = 1.3f;
}
