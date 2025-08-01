// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Threading;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Components;
using Content.Shared.GameTicking.Components;
using Robust.Server.Player;
using Robust.Shared.Player;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server.GameTicking.Rules;

public sealed class InactivityTimeRestartRuleSystem : GameRuleSystem<InactivityRuleComponent>
{
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GameRunLevelChangedEvent>(RunLevelChanged);
        _playerManager.PlayerStatusChanged += PlayerStatusChanged;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _playerManager.PlayerStatusChanged -= PlayerStatusChanged;
    }

    protected override void Ended(EntityUid uid, InactivityRuleComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        StopTimer(uid, component);
    }

    public void RestartTimer(EntityUid uid, InactivityRuleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.TimerCancel.Cancel();
        component.TimerCancel = new CancellationTokenSource();
        Timer.Spawn(component.InactivityMaxTime, () => TimerFired(uid, component), component.TimerCancel.Token);
    }

    public void StopTimer(EntityUid uid, InactivityRuleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        component.TimerCancel.Cancel();
    }

    private void TimerFired(EntityUid uid, InactivityRuleComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;

        GameTicker.EndRound(Loc.GetString("rule-time-has-run-out"));

        _chatManager.DispatchServerAnnouncement(Loc.GetString("rule-restarting-in-seconds", ("seconds",(int) component.RoundEndDelay.TotalSeconds)));

        Timer.Spawn(component.RoundEndDelay, () => GameTicker.RestartRound());
    }

    private void RunLevelChanged(GameRunLevelChangedEvent args)
    {
        var query = EntityQueryEnumerator<InactivityRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var inactivity, out var gameRule))
        {
            if (!GameTicker.IsGameRuleActive(uid, gameRule))
                return;

            switch (args.New)
            {
                case GameRunLevel.InRound:
                    RestartTimer(uid, inactivity);
                    break;
                case GameRunLevel.PreRoundLobby:
                case GameRunLevel.PostRound:
                    StopTimer(uid, inactivity);
                    break;
            }
        }
    }

    private void PlayerStatusChanged(object? sender, SessionStatusEventArgs e)
    {
        var query = EntityQueryEnumerator<InactivityRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var inactivity, out var gameRule))
        {
            if (!GameTicker.IsGameRuleActive(uid, gameRule))
                return;

            if (GameTicker.RunLevel != GameRunLevel.InRound)
            {
                return;
            }

            if (_playerManager.PlayerCount == 0)
            {
                RestartTimer(uid, inactivity);
            }
            else
            {
                StopTimer(uid, inactivity);
            }
        }
    }
}
