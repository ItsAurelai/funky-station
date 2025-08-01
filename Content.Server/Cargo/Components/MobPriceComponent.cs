// SPDX-FileCopyrightText: 2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server.Cargo.Components;

/// <summary>
/// This is used for calculating the price of mobs.
/// </summary>
[RegisterComponent]
public sealed partial class MobPriceComponent : Component
{
    /// <summary>
    /// How much of a penalty per part there should be. This is a multiplier for a multiplier, the penalty for each body part is calculated from the total number of slots, and then multiplied by this.
    /// </summary>
    [DataField("missingBodyPartPenalty")]
    public double MissingBodyPartPenalty = 1.0f;

    /// <summary>
    /// The base price this mob should fetch.
    /// </summary>
    [DataField("price", required: true)]
    public double Price;

    /// <summary>
    /// The percentage of the actual price that should be granted should the appraised mob be dead.
    /// </summary>
    [DataField("deathPenalty")]
    public double DeathPenalty = 0.2f;
}
