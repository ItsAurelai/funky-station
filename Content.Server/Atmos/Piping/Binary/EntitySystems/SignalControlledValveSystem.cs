// SPDX-FileCopyrightText: 2023 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.DeviceLinking.Systems;
using Content.Shared.DeviceLinking.Events;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems;

public sealed class SignalControlledValveSystem : EntitySystem
{
    [Dependency] private readonly DeviceLinkSystem _signal = default!;
    [Dependency] private readonly GasValveSystem _valve = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignalControlledValveComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SignalControlledValveComponent, SignalReceivedEvent>(OnSignalReceived);
    }

    private void OnInit(EntityUid uid, SignalControlledValveComponent comp, ComponentInit args)
    {
        _signal.EnsureSinkPorts(uid, comp.OpenPort, comp.ClosePort, comp.TogglePort);
    }

    private void OnSignalReceived(EntityUid uid, SignalControlledValveComponent comp, ref SignalReceivedEvent args)
    {
        if (!TryComp<GasValveComponent>(uid, out var valve))
            return;

        if (args.Port == comp.OpenPort)
        {
            _valve.Set(uid, valve, true);
        }
        else if (args.Port == comp.ClosePort)
        {
            _valve.Set(uid, valve, false);
        }
        else if (args.Port == comp.TogglePort)
        {
            _valve.Toggle(uid, valve);
        }
    }
}
