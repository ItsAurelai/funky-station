// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 John Space <bigdumb421@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

namespace Content.Server._Goobstation.Flashbang;

[RegisterComponent]
public sealed partial class FlashbangComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float StunTime = 2f;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float KnockdownTime = 10f;
}
