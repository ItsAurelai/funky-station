// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 vulppine <vulppine@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ilya246 <57039557+Ilya246@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 c4llv07e <38111072+c4llv07e@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 eoineoineoin <github@eoinrul.es>
// SPDX-FileCopyrightText: 2025 qwerltaz <msmarcinpl@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor.Components;

[Serializable, NetSerializable]
public enum SharedAirAlarmInterfaceKey
{
    Key
}

[Serializable, NetSerializable]
public enum AirAlarmMode
{
    None,
    Filtering,
    WideFiltering,
    Fill,
    Panic,
}

[Serializable, NetSerializable]
public enum AirAlarmWireStatus
{
    Power,
    Access,
    Panic,
    DeviceSync
}

public interface IAtmosDeviceData
{
    public bool Enabled { get; set; }
    public bool Dirty { get; set; }
    public bool IgnoreAlarms { get; set; }
}

[Serializable, NetSerializable]
public sealed class AirAlarmUIState : BoundUserInterfaceState
{
    public AirAlarmUIState(string address, int deviceCount, float pressureAverage, float temperatureAverage, List<(string, IAtmosDeviceData)> deviceData, AirAlarmMode mode, AtmosAlarmType alarmType, bool autoMode, bool panicWireCut)
    {
        Address = address;
        DeviceCount = deviceCount;
        PressureAverage = pressureAverage;
        TemperatureAverage = temperatureAverage;
        DeviceData = deviceData;
        Mode = mode;
        AlarmType = alarmType;
        AutoMode = autoMode;
        PanicWireCut = panicWireCut;
    }

    public string Address { get; }
    public int DeviceCount { get; }
    public float PressureAverage { get; }
    public float TemperatureAverage { get; }
    /// <summary>
    ///     Every single device data that can be seen from this
    ///     air alarm. This includes vents, scrubbers, and sensors.
    ///     Each entry is a tuple of device address and the device
    ///     data. The same address may appear multiple times, if
    ///     that device provides multiple functions.
    /// </summary>
    public List<(string, IAtmosDeviceData)> DeviceData { get; }
    public AirAlarmMode Mode { get; }
    public AtmosAlarmType AlarmType { get; }
    public bool AutoMode { get; }
    public bool PanicWireCut { get; }
}

[Serializable, NetSerializable]
public sealed class AirAlarmResyncAllDevicesMessage : BoundUserInterfaceMessage
{}

[Serializable, NetSerializable]
public sealed class AirAlarmUpdateAlarmModeMessage : BoundUserInterfaceMessage
{
    public AirAlarmMode Mode { get; }

    public AirAlarmUpdateAlarmModeMessage(AirAlarmMode mode)
    {
        Mode = mode;
    }
}

[Serializable, NetSerializable]
public sealed class AirAlarmUpdateAutoModeMessage : BoundUserInterfaceMessage
{
    public bool Enabled { get; }

    public AirAlarmUpdateAutoModeMessage(bool enabled)
    {
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public sealed class AirAlarmUpdateDeviceDataMessage : BoundUserInterfaceMessage
{
    public string Address { get; }
    public IAtmosDeviceData Data { get; }

    public AirAlarmUpdateDeviceDataMessage(string addr, IAtmosDeviceData data)
    {
        Address = addr;
        Data = data;
    }
}

[Serializable, NetSerializable]
public sealed class AirAlarmCopyDeviceDataMessage : BoundUserInterfaceMessage
{
    public IAtmosDeviceData Data { get; }

    public AirAlarmCopyDeviceDataMessage(IAtmosDeviceData data)
    {
        Data = data;
    }
}

[Serializable, NetSerializable]
public sealed class AirAlarmUpdateAlarmThresholdMessage : BoundUserInterfaceMessage
{
    public string Address { get; }
    public AtmosAlarmThreshold Threshold { get; }
    public AtmosMonitorThresholdType Type { get; }
    public Gas? Gas { get; }

    public AirAlarmUpdateAlarmThresholdMessage(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
    {
        Address = address;
        Threshold = threshold;
        Type = type;
        Gas = gas;
    }
}
