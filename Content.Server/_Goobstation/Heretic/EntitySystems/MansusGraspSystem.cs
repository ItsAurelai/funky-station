// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 duston <66768086+dch-GH@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 jackel234 <52829582+jackel234@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 misghast <51974455+misterghast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server.Chat.Systems;
using Content.Server.EntityEffects.Effects.StatusEffects;
using Content.Server.Heretic.Components;
using Content.Server.Speech.EntitySystems;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.Extensions;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Heretic;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Speech.Muting;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Heretic.EntitySystems;

public sealed partial class MansusGraspSystem : EntitySystem
{
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly RatvarianLanguageSystem _language = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedDoorSystem _door = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffect = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly TemperatureSystem _temperature = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;

    private TimeSpan GraspCooldown { get; } = TimeSpan.FromSeconds(10);

    public void ApplyGraspEffect(EntityUid performer, EntityUid target, string path)
    {
        switch (path)
        {
            case "Ash":
                var timeSpan = TimeSpan.FromSeconds(5f);
                _statusEffect.TryAddStatusEffect(target, TemporaryBlindnessSystem.BlindingStatusEffect, timeSpan, false, TemporaryBlindnessSystem.BlindingStatusEffect);
                break;

            case "Blade":
                // blade is basically an upgrade to the current grasp
                _stamina.TakeStaminaDamage(target, 100f);
                break;

            case "Lock":
                if (!TryComp<DoorComponent>(target, out var door))
                    break;

                if (TryComp<DoorBoltComponent>(target, out var doorBolt))
                    _door.SetBoltsDown((target, doorBolt), false);

                _door.StartOpening(target, door);
                _audio.PlayPvs(new SoundPathSpecifier("/Audio/_Goobstation/Heretic/hereticknock.ogg"), target);
                break;

            case "Flesh":
                if (TryComp<MobStateComponent>(target, out var mobState) && mobState.CurrentState == Shared.Mobs.MobState.Dead)
                {
                    var ghoul = EnsureComp<GhoulComponent>(target);
                    ghoul.BoundHeretic = performer;
                }
                break;

            case "Rust":
                if (!TryComp<DamageableComponent>(target, out var dmg))
                    break;
                // hopefully damage only walls and cyborgs
                if (HasComp<BorgChassisComponent>(target) || !HasComp<StatusEffectsComponent>(target))
                    _damage.SetAllDamage(target, dmg, 50f);
                break;

            case "Void":
                if (TryComp<TemperatureComponent>(target, out var temp))
                    _temperature.ForceChangeTemperature(target, temp.CurrentTemperature - 100f, temp);
                _statusEffect.TryAddStatusEffect<MutedComponent>(target, "Muted", TimeSpan.FromSeconds(8), false);
                break;

            default:
                return;
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MansusGraspComponent, AfterInteractEvent>(OnAfterInteract);

        SubscribeLocalEvent<TagComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<HereticComponent, DrawRitualRuneDoAfterEvent>(OnRitualRuneDoAfter);
    }

    private void OnAfterInteract(Entity<MansusGraspComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach)
            return;

        if (args.Target == null || args.Target == args.User)
            return;

        if (!TryComp<HereticComponent>(args.User, out var hereticComp))
        {
            args.Handled = true;
            QueueDel(ent);
            return;
        }

        var target = (EntityUid) args.Target;

        if ((TryComp<HereticComponent>(args.Target, out var th) && th.CurrentPath == ent.Comp.Path))
            return;

        if (HasComp<StatusEffectsComponent>(target))
        {
            _chat.TrySendInGameICMessage(args.User, Loc.GetString("heretic-speech-mansusgrasp"), InGameICChatType.Speak, false);
            _audio.PlayPvs(new SoundPathSpecifier("/Audio/_Funkystation/Effects/Heretic/mansusgrasp.ogg"), target);
            _stun.TryKnockdown(target, TimeSpan.FromSeconds(3f), true);
            _stamina.TakeStaminaDamage(target, 65f);
            _language.DoRatvarian(target, TimeSpan.FromSeconds(10f), true);

            foreach (var action in _actions.GetActions(args.User))
            {
                if (TryPrototype(action.Id, out var actionPrototype) && actionPrototype.ID.Equals("ActionHereticMansusGrasp"))
                {
                    _actions.SetIfBiggerCooldown(action.Id, GraspCooldown);
                }
            }
        }

        // upgraded grasp
        if (hereticComp.CurrentPath != null)
        {
            if (hereticComp.PathStage >= 2)
                ApplyGraspEffect(args.User, target, hereticComp.CurrentPath!);

            if (hereticComp.PathStage >= 4 && HasComp<StatusEffectsComponent>(target))
            {
                var markComp = EnsureComp<HereticCombatMarkComponent>(target);
                markComp.Path = hereticComp.CurrentPath;
            }
        }

        hereticComp.MansusGraspActive = false;
        args.Handled = true;
        QueueDel(ent);
    }

    private void OnAfterInteract(Entity<TagComponent> ent, ref AfterInteractEvent args)
    {
        var tags = ent.Comp.Tags;

        if (!args.CanReach
        || !args.ClickLocation.IsValid(EntityManager)
        || !TryComp<HereticComponent>(args.User, out var heretic) // not a heretic - how???
        || !heretic.MansusGraspActive // no grasp - not special
        || HasComp<ActiveDoAfterComponent>(args.User) // prevent rune shittery
        || !tags.Contains("Write") || !tags.Contains("Pen")) // not a pen
            return;

        if (args.Target != null && EntityManager.IsChildOf(args.User, args.Target.Value))
            return;

        // remove our rune if clicked
        if (args.Target != null && HasComp<HereticRitualRuneComponent>(args.Target))
        {
            // todo: add more fluff
            args.Handled = true;
            QueueDel(args.Target);
            return;
        }

        // spawn our rune
        var rune = Spawn("HereticRuneRitualDrawAnimation", args.ClickLocation);
        var dargs = new DoAfterArgs(EntityManager, args.User, 14f, new DrawRitualRuneDoAfterEvent(rune, args.ClickLocation), args.User)
        {
            BreakOnDamage = true,
            BreakOnHandChange = true,
            BreakOnMove = true,
            CancelDuplicate = false,
        };
        _doAfter.TryStartDoAfter(dargs);
    }
    private void OnRitualRuneDoAfter(Entity<HereticComponent> ent, ref DrawRitualRuneDoAfterEvent ev)
    {
        // delete the animation rune regardless
        QueueDel(ev.RitualRune);

        if (!ev.Cancelled)
            Spawn("HereticRuneRitual", ev.Coords);
    }
}
