// SPDX-FileCopyrightText: 2023 AJCM <AJCM@tutanota.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Audio;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Damage.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.HotPotato;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server.HotPotato;

public sealed class HotPotatoSystem : SharedHotPotatoSystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly AmbientSoundSystem _ambientSound = default!;
    [Dependency] private readonly DamageOnHoldingSystem _damageOnHolding = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HotPotatoComponent, ActiveTimerTriggerEvent>(OnActiveTimer);
        SubscribeLocalEvent<HotPotatoComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnActiveTimer(EntityUid uid, HotPotatoComponent comp, ref ActiveTimerTriggerEvent args)
    {
        EnsureComp<ActiveHotPotatoComponent>(uid);
        comp.CanTransfer = false;
        _ambientSound.SetAmbience(uid, true);
        _damageOnHolding.SetEnabled(uid, true);
        Dirty(uid, comp);
    }

    private void OnMeleeHit(EntityUid uid, HotPotatoComponent comp, MeleeHitEvent args)
    {
        if (!HasComp<ActiveHotPotatoComponent>(uid))
            return;

        comp.CanTransfer = true;
        foreach (var hitEntity in args.HitEntities)
        {
            if (!TryComp<HandsComponent>(hitEntity, out var hands))
                continue;

            if (!_hands.IsHolding(hitEntity, uid, out _, hands) && _hands.TryForcePickupAnyHand(hitEntity, uid, handsComp: hands))
            {
                _popup.PopupEntity(Loc.GetString("hot-potato-passed",
                    ("from", args.User), ("to", hitEntity)), uid, PopupType.Medium);
                break;
            }

            _popup.PopupEntity(Loc.GetString("hot-potato-failed",
                ("to", hitEntity)), uid, PopupType.Medium);

            break;
        }
        comp.CanTransfer = false;
        Dirty(uid, comp);
    }
}
