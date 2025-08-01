// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Antag.Mimic;
using Content.Server.GameTicking.Rules;
using Content.Server.GameTicking.Rules.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.VendingMachines;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Antag;

public sealed class MobReplacementRuleSystem : GameRuleSystem<MobReplacementRuleComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override void Started(EntityUid uid, MobReplacementRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        var query = AllEntityQuery<VendingMachineComponent, TransformComponent>();
        var spawns = new List<(EntityUid Entity, EntityCoordinates Coordinates)>();

        while (query.MoveNext(out var vendingUid, out _, out var xform))
        {
            if (!_random.Prob(component.Chance))
                continue;

            spawns.Add((vendingUid, xform.Coordinates));
        }

        foreach (var entity in spawns)
        {
            var coordinates = entity.Coordinates;
            Del(entity.Entity);

            Spawn(component.Proto, coordinates);
        }
    }
}
