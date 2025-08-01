// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ilya Chvilyov <90278813+Telyonok@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Climbing.Components
{
    /// <summary>
    /// Indicates this entity can be vaulted on top of.
    /// </summary>
    [RegisterComponent, NetworkedComponent]
    public sealed partial class ClimbableComponent : Component
    {
        /// <summary>
        ///     The range from which this entity can be climbed.
        /// </summary>
        [DataField("range")] public float Range = SharedInteractionSystem.InteractionRange / 1.4f;

        /// <summary>
        ///     The time it takes to climb onto the entity.
        /// </summary>
        [DataField("delay")]
        public float ClimbDelay = 1.5f;

        /// <summary>
        ///     Sound to be played when a climb is started.
        /// </summary>
        [DataField("startClimbSound")]
        public SoundSpecifier? StartClimbSound = null;

        /// <summary>
        ///     Sound to be played when a climb finishes.
        /// </summary>
        [DataField("finishClimbSound")]
        public SoundSpecifier? FinishClimbSound = null;
    }
}
