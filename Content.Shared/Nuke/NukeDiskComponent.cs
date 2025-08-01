// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Nuke;

/// <summary>
/// Used for tracking the nuke disk - isn't a tag for pinpointer purposes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NukeDiskComponent : Component
{
    /// <summary>
    /// Used to modify the nuke's countdown timer.
    /// </summary>
    [DataField]
    public TimeSpan? TimeModifier;

    [DataField]
    public TimeSpan MicrowaveMean = TimeSpan.Zero;

    [DataField]
    public TimeSpan MicrowaveStd = TimeSpan.FromSeconds(27.35);
    // STD of 27.36s means theres an 90% chance the time is between +-45s, and a ~99% chance its between +-70s
}
