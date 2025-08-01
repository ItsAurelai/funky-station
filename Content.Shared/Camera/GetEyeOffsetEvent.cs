// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 QueerCats <jansencheng3@gmail.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;

namespace Content.Shared.Camera;

/// <summary>
///     Raised directed by-ref when <see cref="SharedContentEyeSystem.UpdateEyeOffset"/> is called.
///     Should be subscribed to by any systems that want to modify an entity's eye offset,
///     so that they do not override each other.
/// </summary>
/// <param name="Offset">
///     The total offset to apply.
/// </param>
/// <remarks>
///     Note that in most cases <see cref="Offset"/> should be incremented or decremented by subscribers, not set.
///     Otherwise, any offsets applied by previous subscribing systems will be overridden.
/// </remarks>
[ByRefEvent]
public record struct GetEyeOffsetEvent(Vector2 Offset);

/// <summary>
///     Raised on any equipped and in-hand items that may modify the eye offset.
///     Pockets, tankstorage, and backstorage are excluded.
/// </summary>
[ByRefEvent]
public sealed class GetEyeOffsetRelayedEvent : EntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots { get; } = ~(SlotFlags.POCKET & SlotFlags.TANKSTORAGE & SlotFlags.BACKSTORAGE);

    public Vector2 Offset;
}
