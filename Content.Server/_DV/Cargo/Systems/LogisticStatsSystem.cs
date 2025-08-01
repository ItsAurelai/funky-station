// SPDX-FileCopyrightText: 2024 BombasterDS <115770678+BombasterDS@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 GreyMaria <mariomister541@gmail.com>
// SPDX-FileCopyrightText: 2025 corresp0nd <46357632+corresp0nd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server._DV.Cargo.Components;
using Content.Shared.Cargo;
using JetBrains.Annotations;

namespace Content.Server._DV.Cargo.Systems;

public sealed partial class LogisticStatsSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }

    [PublicAPI]
    public void AddOpenedMailEarnings(EntityUid uid, StationLogisticStatsComponent component, int earnedMoney)
    {
        component.Metrics = component.Metrics with
        {
            Earnings = component.Metrics.Earnings + earnedMoney,
            OpenedCount = component.Metrics.OpenedCount + 1
        };
        UpdateLogisticsStats(uid);
    }

    [PublicAPI]
    public void AddExpiredMailLosses(EntityUid uid, StationLogisticStatsComponent component, int lostMoney)
    {
        component.Metrics = component.Metrics with
        {
            ExpiredLosses = component.Metrics.ExpiredLosses + lostMoney,
            ExpiredCount = component.Metrics.ExpiredCount + 1
        };
        UpdateLogisticsStats(uid);
    }

    [PublicAPI]
    public void AddDamagedMailLosses(EntityUid uid, StationLogisticStatsComponent component, int lostMoney)
    {
        component.Metrics = component.Metrics with
        {
            DamagedLosses = component.Metrics.DamagedLosses + lostMoney,
            DamagedCount = component.Metrics.DamagedCount + 1
        };
        UpdateLogisticsStats(uid);
    }

    [PublicAPI]
    public void AddTamperedMailLosses(EntityUid uid, StationLogisticStatsComponent component, int lostMoney)
    {
        component.Metrics = component.Metrics with
        {
            TamperedLosses = component.Metrics.TamperedLosses + lostMoney,
            TamperedCount = component.Metrics.TamperedCount + 1
        };
        UpdateLogisticsStats(uid);
    }

    private void UpdateLogisticsStats(EntityUid uid) => RaiseLocalEvent(new LogisticStatsUpdatedEvent(uid));
}

public sealed class LogisticStatsUpdatedEvent : EntityEventArgs
{
    public EntityUid Station;
    public LogisticStatsUpdatedEvent(EntityUid station)
    {
        Station = station;
    }
}
