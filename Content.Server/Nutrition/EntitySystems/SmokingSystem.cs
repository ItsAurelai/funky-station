// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2022 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 TekuNut <13456422+TekuNut@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 brainfood1183 <113240905+brainfood1183@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 iller_saver <55444968+illersaver@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Cojoke <83733158+Cojoke-dot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 empty0set <16693552+empty0set@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 empty0set <empty0set@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Shared.Chemistry.EntitySystems;
using Content.Server.Forensics;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Nutrition.Components;
using Content.Shared.Smoking;
using Content.Shared.Temperature;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using System.Linq;
using Content.Shared.Bed.Sleep;
using Content.Shared.Buckle.Components;
using Robust.Shared.Random;
using Content.Shared.Atmos;

namespace Content.Server.Nutrition.EntitySystems
{
    public sealed partial class SmokingSystem : EntitySystem
    {
        [Dependency] private readonly ReactiveSystem _reactiveSystem = default!;
        [Dependency] private readonly SharedSolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
        [Dependency] private readonly AtmosphereSystem _atmos = default!;
        [Dependency] private readonly TransformSystem _transformSystem = default!;
        [Dependency] private readonly InventorySystem _inventorySystem = default!;
        [Dependency] private readonly ClothingSystem _clothing = default!;
        [Dependency] private readonly SharedItemSystem _items = default!;
        [Dependency] private readonly SharedContainerSystem _container = default!;
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly ForensicsSystem _forensics = default!;
        [Dependency] private readonly FlammableSystem _flammableSystem = default!; // Funkystation
        [Dependency] private readonly IRobustRandom _random = default!; // Funkystation

        private const float UpdateTimer = 3f;

        private float _timer;
        /// <summary>
        ///     We keep a list of active smokables, because iterating all existing smokables would be dumb.
        /// </summary>
        private readonly HashSet<EntityUid> _active = new();

        public override void Initialize()
        {
            SubscribeLocalEvent<SmokableComponent, IsHotEvent>(OnSmokableIsHotEvent);
            SubscribeLocalEvent<SmokableComponent, ComponentShutdown>(OnSmokableShutdownEvent);
            SubscribeLocalEvent<SmokableComponent, GotEquippedEvent>(OnSmokeableEquipEvent);
            Subs.SubscribeWithRelay<SmokableComponent, ExtinguishEvent>(OnExtinguishEvent);

            InitializeCigars();
            InitializePipes();
            InitializeVapes();
        }

        private void OnExtinguishEvent(Entity<SmokableComponent> ent, ref ExtinguishEvent args)
        {
            if (ent.Comp.State == SmokableState.Lit)
                SetSmokableState(ent, SmokableState.Burnt, ent);
        }

        public void SetSmokableState(EntityUid uid, SmokableState state, SmokableComponent? smokable = null,
            AppearanceComponent? appearance = null, ClothingComponent? clothing = null)
        {
            if (!Resolve(uid, ref smokable, ref appearance, ref clothing))
                return;

            smokable.State = state;
            _appearance.SetData(uid, SmokingVisuals.Smoking, state, appearance);

            var newState = state switch
            {
                SmokableState.Lit => smokable.LitPrefix,
                SmokableState.Burnt => smokable.BurntPrefix,
                _ => smokable.UnlitPrefix
            };

            _clothing.SetEquippedPrefix(uid, newState, clothing);
            _items.SetHeldPrefix(uid, newState);

            if (state == SmokableState.Lit)
            {
                var igniteEvent = new IgnitedEvent();
                RaiseLocalEvent(uid, ref igniteEvent);

                _active.Add(uid);
            }
            else
            {
                var igniteEvent = new ExtinguishedEvent();
                RaiseLocalEvent(uid, ref igniteEvent);

                _active.Remove(uid);
            }
        }

        private void OnSmokableIsHotEvent(Entity<SmokableComponent> entity, ref IsHotEvent args)
        {
            args.IsHot = entity.Comp.State == SmokableState.Lit;
        }

        private void OnSmokableShutdownEvent(Entity<SmokableComponent> entity, ref ComponentShutdown args)
        {
            _active.Remove(entity);
        }

        private void OnSmokeableEquipEvent(Entity<SmokableComponent> entity, ref GotEquippedEvent args)
        {
            if (args.Slot == "mask")
            {
                _forensics.TransferDna(entity.Owner, args.Equipee, false);
            }
        }

        public override void Update(float frameTime)
        {
            _timer += frameTime;

            if (_timer < UpdateTimer)
                return;

            // TODO Use an "active smoke" component instead, EntityQuery over that.
            foreach (var uid in _active.ToArray())
            {
                if (!TryComp(uid, out SmokableComponent? smokable))
                {
                    _active.Remove(uid);
                    continue;
                }

                if (!_solutionContainerSystem.TryGetSolution(uid, smokable.Solution, out var soln, out var solution))
                {
                    _active.Remove(uid);
                    continue;
                }

                if (smokable.ExposeTemperature > 0 && smokable.ExposeVolume > 0)
                {
                    var transform = Transform(uid);

                    if (transform.GridUid is { } gridUid)
                    {
                        var position = _transformSystem.GetGridOrMapTilePosition(uid, transform);
                        _atmos.HotspotExpose(gridUid, position, smokable.ExposeTemperature, smokable.ExposeVolume, uid, true);
                    }
                }

                var inhaledSolution = _solutionContainerSystem.SplitSolution(soln.Value, smokable.InhaleAmount * _timer);

                if (solution.Volume == FixedPoint2.Zero)
                {
                    RaiseLocalEvent(uid, new SmokableSolutionEmptyEvent(), true);
                }

                if (inhaledSolution.Volume == FixedPoint2.Zero)
                    continue;

                // This is awful. I hate this so much.
                // TODO: Please, someone refactor containers and free me from this bullshit.
                if (!_container.TryGetContainingContainer((uid, null, null), out var containerManager) ||
                    !(_inventorySystem.TryGetSlotEntity(containerManager.Owner, "mask", out var inMaskSlotUid) && inMaskSlotUid == uid) ||
                    !TryComp(containerManager.Owner, out BloodstreamComponent? bloodstream))
                {
                    continue;
                }

                // BEGIN Funkystation
                // Smoking in bed is dangerous!
                if (HasComp<SleepingComponent>(containerManager.Owner)
                    && HasComp<BuckleComponent>(containerManager.Owner))
                {
                    // 25% chance over the lifetime of a cigarette (66 times)
                    if (_random.Prob(0.03f))
                        _flammableSystem.AdjustFireStacks(containerManager.Owner, 0.5f, null, true);
                }
                // END Funkystation

                _reactiveSystem.DoEntityReaction(containerManager.Owner, inhaledSolution, ReactionMethod.Ingestion);
                _bloodstreamSystem.TryAddToChemicals(containerManager.Owner, inhaledSolution, bloodstream);
            }

            _timer -= UpdateTimer;
        }
    }

    /// <summary>
    ///     Directed event raised when the smokable solution is empty.
    /// </summary>
    public sealed class SmokableSolutionEmptyEvent : EntityEventArgs
    {
    }
}
