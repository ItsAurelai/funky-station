// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tornado Tech <54727692+Tornado-Technology@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Threading;
using System.Threading.Tasks;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Math;

/// <summary>
/// Set <see cref="SetFloatOperator.Amount"/> to float value for the
/// specified <see cref="SetFloatOperator.TargetKey"/> in the <see cref="NPCBlackboard"/>.
/// </summary>
public sealed partial class SetFloatOperator : HTNOperator
{
    [DataField(required: true), ViewVariables]
    public string TargetKey = string.Empty;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Amount;

    public override async Task<(bool Valid, Dictionary<string, object>? Effects)> Plan(NPCBlackboard blackboard,
        CancellationToken cancelToken)
    {
        return (
            true,
            new Dictionary<string, object>
            {
                { TargetKey, Amount }
            }
        );
    }
}
