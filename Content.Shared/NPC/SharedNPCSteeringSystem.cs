// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.NPC;

public abstract class SharedNPCSteeringSystem : EntitySystem
{
    public const byte InterestDirections = 12;

    /// <summary>
    /// How many radians between each interest direction.
    /// </summary>
    public const float InterestRadians = MathF.Tau / InterestDirections;

    /// <summary>
    /// How many degrees between each interest direction.
    /// </summary>
    public const float InterestDegrees = 360f / InterestDirections;
}
