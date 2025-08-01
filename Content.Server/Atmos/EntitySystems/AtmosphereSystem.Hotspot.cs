// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2022 Kevin Zheng <kevinz5000@gmail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 marc-pelletier <113944176+marc-pelletier@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Components;
using Content.Server.Decals;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Reactions;
using Content.Shared.Database;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server.Atmos.EntitySystems
{
    public sealed partial class AtmosphereSystem
    {
        [Dependency] private readonly DecalSystem _decalSystem = default!;
        [Dependency] private readonly IRobustRandom _random = default!;

        private const int HotspotSoundCooldownCycles = 200;

        private int _hotspotSoundCooldown = 0;

        [ViewVariables(VVAccess.ReadWrite)]
        public string? HotspotSound { get; private set; } = "/Audio/Effects/fire.ogg";

        private void ProcessHotspot(
            Entity<GridAtmosphereComponent, GasTileOverlayComponent, MapGridComponent, TransformComponent> ent,
            TileAtmosphere tile)
        {
            var gridAtmosphere = ent.Comp1;
            if (!tile.Hotspot.Valid)
            {
                gridAtmosphere.HotspotTiles.Remove(tile);
                return;
            }

            AddActiveTile(gridAtmosphere, tile);

            if (!tile.Hotspot.SkippedFirstProcess)
            {
                tile.Hotspot.SkippedFirstProcess = true;
                return;
            }

            if(tile.ExcitedGroup != null)
                ExcitedGroupResetCooldowns(tile.ExcitedGroup);

            if ((tile.Hotspot.Temperature < Atmospherics.FireMinimumTemperatureToExist) || (tile.Hotspot.Volume <= 1f)
               || (tile.Air == null || tile.Air.GetMoles(Gas.Oxygen) < 0.5f || (tile.Air.GetMoles(Gas.Plasma) < 0.5f
               && tile.Air.GetMoles(Gas.Tritium) < 0.5f && tile.Air.GetMoles(Gas.Hydrogen) < 0.5f)
               || tile.Air.GetMoles(Gas.HyperNoblium) > 5f))  // Assmos - /tg/ gases
            {
                tile.Hotspot = new Hotspot();
                InvalidateVisuals(ent, tile);
                return;
            }

            PerformHotspotExposure(tile);

            if (tile.Hotspot.Bypassing)
            {
                tile.Hotspot.State = 3;

                var gridUid = ent.Owner;
                var tilePos = tile.GridIndices;

                // Get the existing decals on the tile
                var tileDecals = _decalSystem.GetDecalsInRange(gridUid, tilePos);

                // Count the burnt decals on the tile
                var tileBurntDecals = 0;

                foreach (var set in tileDecals)
                {
                    if (Array.IndexOf(_burntDecals, set.Decal.Id) == -1)
                        continue;

                    tileBurntDecals++;

                    if (tileBurntDecals > 4)
                        break;
                }

                // Add a random burned decal to the tile only if there are less than 4 of them
                if (tileBurntDecals < 4)
                    _decalSystem.TryAddDecal(_burntDecals[_random.Next(_burntDecals.Length)], new EntityCoordinates(gridUid, tilePos), out _, cleanable: true);

                if (tile.Air.Temperature > Atmospherics.FireMinimumTemperatureToSpread)
                {
                    var radiatedTemperature = tile.Air.Temperature * Atmospherics.FireSpreadRadiosityScale;
                    foreach (var otherTile in tile.AdjacentTiles)
                    {
                        // TODO ATMOS: This is sus. Suss this out.
                        if (otherTile == null)
                            continue;

                        if(!otherTile.Hotspot.Valid)
                            HotspotExpose(gridAtmosphere, otherTile, radiatedTemperature, Atmospherics.CellVolume/4);
                    }
                }
            }
            else
            {
                tile.Hotspot.State = (byte) (tile.Hotspot.Volume > Atmospherics.CellVolume * 0.4f ? 2 : 1);
            }

            if (tile.Hotspot.Temperature > tile.MaxFireTemperatureSustained)
                tile.MaxFireTemperatureSustained = tile.Hotspot.Temperature;

            if (_hotspotSoundCooldown++ == 0 && !string.IsNullOrEmpty(HotspotSound))
            {
                var coordinates = _mapSystem.ToCenterCoordinates(tile.GridIndex, tile.GridIndices);

                // A few details on the audio parameters for fire.
                // The greater the fire state, the lesser the pitch variation.
                // The greater the fire state, the greater the volume.
                _audio.PlayPvs(HotspotSound, coordinates, AudioParams.Default.WithVariation(0.15f/tile.Hotspot.State).WithVolume(-5f + 5f * tile.Hotspot.State));
            }

            if (_hotspotSoundCooldown > HotspotSoundCooldownCycles)
                _hotspotSoundCooldown = 0;

            // TODO ATMOS Maybe destroy location here?
        }

        private void HotspotExpose(GridAtmosphereComponent gridAtmosphere, TileAtmosphere tile,
            float exposedTemperature, float exposedVolume, bool soh = false, EntityUid? sparkSourceUid = null)
        {
            if (tile.Air == null)
                return;

            var oxygen = tile.Air.GetMoles(Gas.Oxygen);

            if (oxygen < 0.5f)
                return;

            var plasma = tile.Air.GetMoles(Gas.Plasma);
            var tritium = tile.Air.GetMoles(Gas.Tritium);
            var hydrogen = tile.Air.GetMoles(Gas.Hydrogen); // Assmos - /tg/ gases
            var hypernob = tile.Air.GetMoles(Gas.HyperNoblium); // Assmos - /tg/ gases

            if (tile.Hotspot.Valid)
            {
                if (soh)
                {
                    if (plasma > 0.5f && hypernob < 5f || tritium > 0.5f && hypernob < 5f || hydrogen > 0.5f && hypernob < 5f) // Assmos - /tg/ gases
                    {
                        if (tile.Hotspot.Temperature < exposedTemperature)
                            tile.Hotspot.Temperature = exposedTemperature;
                        if (tile.Hotspot.Volume < exposedVolume)
                            tile.Hotspot.Volume = exposedVolume;
                    }
                }

                return;
            }

            if ((exposedTemperature > Atmospherics.PlasmaMinimumBurnTemperature) && (plasma > 0.5f && hypernob < 5f || tritium > 0.5f && hypernob < 5f || hydrogen > 0.5f && hypernob < 5f)) // Assmos - /tg/ gases
            {
                if (sparkSourceUid.HasValue)
                    _adminLog.Add(LogType.Flammable, LogImpact.High, $"Heat/spark of {ToPrettyString(sparkSourceUid.Value)} caused atmos ignition of gas: {tile.Air.Temperature.ToString():temperature}K - {oxygen}mol Oxygen, {plasma}mol Plasma, {tritium}mol Tritium, {hydrogen}mol Hydrogen"); //Assmos - /tg/ gases

                tile.Hotspot = new Hotspot
                {
                    Volume = exposedVolume * 25f,
                    Temperature = exposedTemperature,
                    SkippedFirstProcess = tile.CurrentCycle > gridAtmosphere.UpdateCounter,
                    Valid = true,
                    State = 1
                };

                AddActiveTile(gridAtmosphere, tile);
                gridAtmosphere.HotspotTiles.Add(tile);
            }
        }

        private void PerformHotspotExposure(TileAtmosphere tile)
        {
            if (tile.Air == null || !tile.Hotspot.Valid) return;

            tile.Hotspot.Bypassing = tile.Hotspot.SkippedFirstProcess && tile.Hotspot.Volume > tile.Air.Volume*0.95f;

            if (tile.Hotspot.Bypassing)
            {
                tile.Hotspot.Volume = tile.Air.ReactionResults[(byte)GasReaction.Fire] * Atmospherics.FireGrowthRate;
                tile.Hotspot.Temperature = tile.Air.Temperature;
            }
            else
            {
                var affected = tile.Air.RemoveVolume(tile.Hotspot.Volume);
                affected.Temperature = tile.Hotspot.Temperature;
                React(affected, tile);
                tile.Hotspot.Temperature = affected.Temperature;
                tile.Hotspot.Volume = affected.ReactionResults[(byte)GasReaction.Fire] * Atmospherics.FireGrowthRate;
                Merge(tile.Air, affected);
            }

            var fireEvent = new TileFireEvent(tile.Hotspot.Temperature, tile.Hotspot.Volume);
            _entSet.Clear();
            _lookup.GetLocalEntitiesIntersecting(tile.GridIndex, tile.GridIndices, _entSet, 0f);

            foreach (var entity in _entSet)
            {
                RaiseLocalEvent(entity, ref fireEvent);
            }
        }
    }
}
