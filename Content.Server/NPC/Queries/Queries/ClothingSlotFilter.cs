// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Inventory;

namespace Content.Server.NPC.Queries.Queries;

public sealed partial class ClothingSlotFilter : UtilityQueryFilter
{
    [DataField("slotFlags", required: true)]
    public SlotFlags SlotFlags = SlotFlags.NONE;
}
