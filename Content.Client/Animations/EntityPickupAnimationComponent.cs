// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Client.Animations;

/// <summary>
///     Applied to client-side clone entities to animate them approaching the player that
///     picked up the original entity.
/// </summary>
[RegisterComponent]
[Access(typeof(EntityPickupAnimationSystem))]
public sealed partial class EntityPickupAnimationComponent : Component
{
}
