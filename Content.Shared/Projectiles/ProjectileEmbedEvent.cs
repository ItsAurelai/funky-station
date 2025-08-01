// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Dakamakat <52600490+dakamakat@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.Projectiles;

/// <summary>
/// Raised directed on an entity when it embeds into something.
/// </summary>
[ByRefEvent]
public readonly record struct ProjectileEmbedEvent(EntityUid? Shooter, EntityUid Weapon, EntityUid Embedded);
