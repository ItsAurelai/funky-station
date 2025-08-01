// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 August Eymann <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2024 marc-pelletier <113944176+marc-pelletier@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Charges.Systems;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared.Charges.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedChargesSystem))]
[AutoGenerateComponentState]
public sealed partial class LimitedChargesComponent : Component
{
    /// <summary>
    /// The maximum number of charges
    /// </summary>
    [DataField("maxCharges"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public FixedPoint2 MaxCharges = 6;

    /// <summary>
    /// The current number of charges
    /// </summary>
    [DataField("charges"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public FixedPoint2 Charges = 6;
}
