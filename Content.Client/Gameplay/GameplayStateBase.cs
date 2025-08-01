// SPDX-FileCopyrightText: 2020 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2020 ComicIronic <comicironic@gmail.com>
// SPDX-FileCopyrightText: 2020 F77F <66768086+F77F@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Sam <Sam.V_@hotmail.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2020 chairbender <kwhipke1@gmail.com>
// SPDX-FileCopyrightText: 2020 scuffedjays <yetanotherscuffed@gmail.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Numerics;
using Content.Client.Clickable;
using Content.Client.UserInterface;
using Content.Client.Viewport;
using Content.Shared.Input;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using YamlDotNet.Serialization.TypeInspectors;

namespace Content.Client.Gameplay
{
    // OH GOD.
    // Ok actually it's fine.
    // Instantiated dynamically through the StateManager, Dependencies will be resolved.
    [Virtual]
    public class GameplayStateBase : State, IEntityEventSubscriber
    {
        [Dependency] private readonly IEyeManager _eyeManager = default!;
        [Dependency] private readonly IInputManager _inputManager = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] protected readonly IUserInterfaceManager UserInterfaceManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IViewVariablesManager _vvm = default!;
        [Dependency] private readonly IConsoleHost _conHost = default!;

        private ClickableEntityComparer _comparer = default!;

        private (ViewVariablesPath? path, string[] segments) ResolveVvHoverObject(string path)
        {
            var segments = path.Split('/');
            var uid = RecursivelyFindUiEntity(UserInterfaceManager.CurrentlyHovered);
            var netUid = _entityManager.GetNetEntity(uid);
            return (netUid != null ? new ViewVariablesInstancePath(netUid) : null, segments);
        }

        private EntityUid? RecursivelyFindUiEntity(Control? control)
        {
            if (control == null)
                return null;

            switch (control)
            {
                case IViewportControl vp:
                    if (_inputManager.MouseScreenPosition.IsValid)
                        return GetClickedEntity(vp.PixelToMap(_inputManager.MouseScreenPosition.Position));
                    return null;
                case SpriteView sprite:
                    return sprite.Entity;
                case IEntityControl ui:
                    return ui.UiEntity;
            }

            return RecursivelyFindUiEntity(control.Parent);
        }

        private IEnumerable<string>? ListVVHoverPaths(string[] segments)
        {
            return null;
        }

        protected override void Startup()
        {
            _vvm.RegisterDomain("enthover", ResolveVvHoverObject, ListVVHoverPaths);
            _inputManager.KeyBindStateChanged += OnKeyBindStateChanged;
            _comparer = new ClickableEntityComparer();
            CommandBinds.Builder
                .Bind(ContentKeyFunctions.InspectEntity, new PointerInputCmdHandler(HandleInspect, outsidePrediction: true))
                .Register<GameplayStateBase>();
        }

        protected override void Shutdown()
        {
            _vvm.UnregisterDomain("enthover");
            _inputManager.KeyBindStateChanged -= OnKeyBindStateChanged;
            CommandBinds.Unregister<GameplayStateBase>();
        }

        private bool HandleInspect(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            _conHost.ExecuteCommand($"vv /c/enthover");
            return true;
        }

        public EntityUid? GetClickedEntity(MapCoordinates coordinates)
        {
            return GetClickedEntity(coordinates, _eyeManager.CurrentEye);
        }

        public EntityUid? GetClickedEntity(MapCoordinates coordinates, IEye? eye)
        {
            if (eye == null)
                return null;

            var first = GetClickableEntities(coordinates, eye).FirstOrDefault();
            return first.IsValid() ? first : null;
        }

        public IEnumerable<EntityUid> GetClickableEntities(EntityCoordinates coordinates, bool excludeFaded = true)
        {
            var transformSystem = _entitySystemManager.GetEntitySystem<SharedTransformSystem>();
            return GetClickableEntities(transformSystem.ToMapCoordinates(coordinates), excludeFaded);
        }

        public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates, bool excludeFaded = true)
        {
            return GetClickableEntities(coordinates, _eyeManager.CurrentEye, excludeFaded);
        }

        public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates, IEye? eye, bool excludeFaded = true)
        {
            /*
             * TODO:
             * 1. Stuff like MeleeWeaponSystem need an easy way to hook into viewport specific entities / entities under mouse
             * 2. Cleanup the mess around InteractionOutlineSystem + below the keybind click detection.
             */

            if (eye == null)
                return Array.Empty<EntityUid>();

            // Find all the entities intersecting our click
            var spriteTree = _entityManager.EntitySysManager.GetEntitySystem<SpriteTreeSystem>();
            var entities = spriteTree.QueryAabb(coordinates.MapId, Box2.CenteredAround(coordinates.Position, new Vector2(1, 1)));

            // Check the entities against whether or not we can click them
            var foundEntities = new List<(EntityUid, int, uint, float)>(entities.Count);
            var clickQuery = _entityManager.GetEntityQuery<ClickableComponent>();
            var clickables = _entityManager.System<ClickableSystem>();

            foreach (var entity in entities)
            {
                if (clickQuery.TryGetComponent(entity.Uid, out var component) &&
                    clickables.CheckClick((entity.Uid, component, entity.Component, entity.Transform), coordinates.Position, eye, excludeFaded, out var drawDepthClicked, out var renderOrder, out var bottom))
                {
                    foundEntities.Add((entity.Uid, drawDepthClicked, renderOrder, bottom));
                }
            }

            if (foundEntities.Count == 0)
                return Array.Empty<EntityUid>();

            // Do drawdepth & y-sorting. First index is the top-most sprite (opposite of normal render order).
            foundEntities.Sort(_comparer);

            return foundEntities.Select(a => a.Item1);
        }

        private sealed class ClickableEntityComparer : IComparer<(EntityUid clicked, int depth, uint renderOrder, float bottom)>
        {
            public int Compare((EntityUid clicked, int depth, uint renderOrder, float bottom) x,
                (EntityUid clicked, int depth, uint renderOrder, float bottom) y)
            {
                var cmp = y.depth.CompareTo(x.depth);
                if (cmp != 0)
                {
                    return cmp;
                }

                cmp = y.renderOrder.CompareTo(x.renderOrder);

                if (cmp != 0)
                {
                    return cmp;
                }

                cmp = -y.bottom.CompareTo(x.bottom);

                if (cmp != 0)
                {
                    return cmp;
                }

                return y.clicked.CompareTo(x.clicked);
            }
        }

        /// <summary>
        ///     Converts a state change event from outside the simulation to inside the simulation.
        /// </summary>
        /// <param name="args">Event data values for a bound key state change.</param>
        protected virtual void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
        {
            // If there is no InputSystem, then there is nothing to forward to, and nothing to do here.
            if(!_entitySystemManager.TryGetEntitySystem(out InputSystem? inputSys))
                return;

            var kArgs = args.KeyEventArgs;
            var func = kArgs.Function;
            var funcId = _inputManager.NetworkBindMap.KeyFunctionID(func);

            EntityCoordinates coordinates = default;
            EntityUid? entityToClick = null;
            if (args.Viewport is IViewportControl vp && kArgs.PointerLocation.IsValid)
            {
                var mousePosWorld = vp.PixelToMap(kArgs.PointerLocation.Position);

                if (vp is ScalingViewport svp)
                {
                    entityToClick = GetClickedEntity(mousePosWorld, svp.Eye);
                }
                else
                {
                    entityToClick = GetClickedEntity(mousePosWorld);
                }
                var transformSystem = _entitySystemManager.GetEntitySystem<SharedTransformSystem>();
                var mapSystem = _entitySystemManager.GetEntitySystem<MapSystem>();

                coordinates = _mapManager.TryFindGridAt(mousePosWorld, out var uid, out _) ?
                    mapSystem.MapToGrid(uid, mousePosWorld) :
                    transformSystem.ToCoordinates(mousePosWorld);
            }
            else
            {
                coordinates = EntityCoordinates.Invalid;
            }

            var message = new ClientFullInputCmdMessage(_timing.CurTick, _timing.TickFraction, funcId)
            {
                State = kArgs.State,
                Coordinates = coordinates,
                ScreenCoordinates = kArgs.PointerLocation,
                Uid = entityToClick ?? default,
            }; // TODO make entityUid nullable

            // client side command handlers will always be sent the local player session.
            var session = _playerManager.LocalSession;
            if (inputSys.HandleInputCommand(session, func, message))
            {
                kArgs.Handle();
            }
        }
    }
}
