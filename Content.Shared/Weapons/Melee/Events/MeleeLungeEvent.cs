// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 I.K <45953835+notquitehadouken@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 notquitehadouken <1isthisameme>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared.Weapons.Melee.Events;

/// <summary>
/// Data for melee lunges from attacks.
/// </summary>
[Serializable, NetSerializable]
public sealed class MeleeLungeEvent : EntityEventArgs
{
    public NetEntity Entity;

    /// <summary>
    /// The weapon used.
    /// </summary>
    public NetEntity Weapon;

    /// <summary>
    /// Width of the attack angle.
    /// </summary>
    public Angle Angle;

    /// <summary>
    /// The relative local position to the <see cref="Entity"/>
    /// </summary>
    public Vector2 LocalPos;

    /// <summary>
    /// Entity to spawn for the animation
    /// </summary>
    public string? Animation;

    public MeleeLungeEvent(NetEntity entity, NetEntity weapon, Angle angle, Vector2 localPos, string? animation)
    {
        Entity = entity;
        Weapon = weapon;
        Angle = angle;
        LocalPos = localPos;
        Animation = animation;
    }
}
