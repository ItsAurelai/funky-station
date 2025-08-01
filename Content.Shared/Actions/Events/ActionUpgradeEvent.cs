// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Shared.Actions.Events;

public sealed class ActionUpgradeEvent : EntityEventArgs
{
    public int NewLevel;
    public EntityUid? ActionId;

    public ActionUpgradeEvent(int newLevel, EntityUid? actionId)
    {
        NewLevel = newLevel;
        ActionId = actionId;
    }
}
