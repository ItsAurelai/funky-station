// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 vulppine <vulppine@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2025 marc-pelletier <113944176+marc-pelletier@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 qwerltaz <msmarcinpl@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos.Monitor.Components;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components
{
    [Serializable, NetSerializable]
    public sealed class GasVentScrubberData : IAtmosDeviceData
    {
        public bool Enabled { get; set; }
        public bool Dirty { get; set; }
        public bool IgnoreAlarms { get; set; } = false;
        public HashSet<Gas> FilterGases { get; set; } = new(DefaultFilterGases);
        public ScrubberPumpDirection PumpDirection { get; set; } = ScrubberPumpDirection.Scrubbing;
        public float VolumeRate { get; set; } = 200f;
        public bool WideNet { get; set; } = false;
        public bool AirAlarmPanicWireCut { get; set; }

        public static HashSet<Gas> DefaultFilterGases = new()
        {
            Gas.CarbonDioxide,
            Gas.Plasma,
            Gas.Tritium,
            Gas.WaterVapor,
            Gas.Ammonia,
            Gas.NitrousOxide,
            Gas.Frezon,
            Gas.BZ, // Assmos - /tg/ gases
            Gas.Healium, // Assmos - /tg/ gases
            Gas.Nitrium, // Assmos - /tg/ gases
            Gas.Hydrogen, // Assmos - /tg/ gases
            Gas.HyperNoblium, // Assmos - /tg/ gases
            Gas.ProtoNitrate, // Assmos - /tg/ gases
            Gas.Zauker, // Assmos - /tg/ gases
            Gas.Halon, // Assmos - /tg/ gases
            Gas.Helium, // Assmos - /tg/ gases
            Gas.AntiNoblium, // Assmos - /tg/ gases
        };

        // Presets for 'dumb' air alarm modes

        public static GasVentScrubberData FilterModePreset = new GasVentScrubberData
        {
            Enabled = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = false
        };

        public static GasVentScrubberData WideFilterModePreset = new GasVentScrubberData
        {
            Enabled = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = true
        };

        public static GasVentScrubberData FillModePreset = new GasVentScrubberData
        {
            Enabled = false,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Scrubbing,
            VolumeRate = 200f,
            WideNet = false
        };

        public static GasVentScrubberData PanicModePreset = new GasVentScrubberData
        {
            Enabled = true,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Siphoning,
            VolumeRate = 200f,
            WideNet = true
        };

        public static GasVentScrubberData ReplaceModePreset = new GasVentScrubberData
        {
            Enabled = true,
            IgnoreAlarms = true,
            Dirty = true,
            FilterGases = new(GasVentScrubberData.DefaultFilterGases),
            PumpDirection = ScrubberPumpDirection.Siphoning,
            VolumeRate = 200f,
            WideNet = false
        };
    }

    [Serializable, NetSerializable]
    public enum ScrubberPumpDirection : sbyte
    {
        Siphoning = 0,
        Scrubbing = 1,
    }
}
