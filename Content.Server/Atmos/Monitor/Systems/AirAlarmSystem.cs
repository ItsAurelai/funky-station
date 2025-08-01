// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 corentt <36075110+corentt@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 vulppine <vulppine@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ilya246 <57039557+Ilya246@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Julian Giebel <juliangiebel@live.de>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 c4llv07e <38111072+c4llv07e@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 eoineoineoin <github@eoinrul.es>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 qwerltaz <msmarcinpl@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.Monitor.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.DeviceLinking.Systems;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Database;
using Content.Shared.DeviceLinking;
using Content.Shared.DeviceNetwork;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Interaction;
using Content.Shared.Power;
using Content.Shared.Wires;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using System.Linq;

namespace Content.Server.Atmos.Monitor.Systems;

// AirAlarm system - specific for atmos devices, rather than
// atmos monitors.
//
// oh boy, message passing!
//
// Commands should always be sent into packet's Command
// data key. In response, a packet will be transmitted
// with the response type as its command, and the
// response data in its data key.
public sealed class AirAlarmSystem : EntitySystem
{
    [Dependency] private readonly AccessReaderSystem _access = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AtmosAlarmableSystem _atmosAlarmable = default!;
    [Dependency] private readonly AtmosDeviceNetworkSystem _atmosDevNet = default!;
    [Dependency] private readonly DeviceNetworkSystem _deviceNet = default!;
    [Dependency] private readonly DeviceLinkSystem _deviceLink = default!;
    [Dependency] private readonly DeviceListSystem _deviceList = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    #region Device Network API

    /// <summary>
    ///     Command to set an air alarm's mode.
    /// </summary>
    public const string AirAlarmSetMode = "air_alarm_set_mode";

    // -- API --

    /// <summary>
    ///     Set the data for an air alarm managed device.
    /// </summary>
    /// <param name="address">The address of the device.</param>
    /// <param name="data">The data to send to the device.</param>
    public void SetData(EntityUid uid, string address, IAtmosDeviceData data)
    {
        _atmosDevNet.SetDeviceState(uid, address, data);
        _atmosDevNet.Sync(uid, address);
    }

    /// <summary>
    ///     Broadcast a sync packet to an air alarm's local network.
    /// </summary>
    private void SyncAllDevices(EntityUid uid)
    {
        _atmosDevNet.Sync(uid, null);
    }

    /// <summary>
    ///     Send a sync packet to a specific device from an air alarm.
    /// </summary>
    /// <param name="address">The address of the device.</param>
    private void SyncDevice(EntityUid uid, string address)
    {
        _atmosDevNet.Sync(uid, address);
    }

    /// <summary>
    ///     Register and synchronize with all devices
    ///     on this network.
    /// </summary>
    /// <param name="uid"></param>
    private void SyncRegisterAllDevices(EntityUid uid)
    {
        _atmosDevNet.Register(uid, null);
        _atmosDevNet.Sync(uid, null);
    }

    /// <summary>
    ///     Synchronize all sensors on an air alarm, but only if its current tab is set to Sensors.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="monitor"></param>
    private void SyncAllSensors(EntityUid uid, AirAlarmComponent? monitor = null)
    {
        if (!Resolve(uid, ref monitor))
        {
            return;
        }

        foreach (var addr in monitor.SensorData.Keys)
        {
            SyncDevice(uid, addr);
        }
    }

    private void SetThreshold(EntityUid uid, string address, AtmosMonitorThresholdType type,
        AtmosAlarmThreshold threshold, Gas? gas = null)
    {
        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = AtmosMonitorSystem.AtmosMonitorSetThresholdCmd,
            [AtmosMonitorSystem.AtmosMonitorThresholdDataType] = type,
            [AtmosMonitorSystem.AtmosMonitorThresholdData] = threshold,
        };

        if (gas != null)
        {
            payload.Add(AtmosMonitorSystem.AtmosMonitorThresholdGasType, gas);
        }

        _deviceNet.QueuePacket(uid, address, payload);

        SyncDevice(uid, address);
    }

    private void SetAllThresholds(EntityUid uid, string address, AtmosSensorData data)
    {
        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = AtmosMonitorSystem.AtmosMonitorSetAllThresholdsCmd,
            [AtmosMonitorSystem.AtmosMonitorAllThresholdData] = data
        };

        _deviceNet.QueuePacket(uid, address, payload);

        SyncDevice(uid, address);
    }

    /// <summary>
    ///     Sync this air alarm's mode with the rest of the network.
    /// </summary>
    /// <param name="mode">The mode to sync with the rest of the network.</param>
    private void SyncMode(EntityUid uid, AirAlarmMode mode)
    {
        if (TryComp<AtmosMonitorComponent>(uid, out var monitor) && !monitor.NetEnabled)
            return;

        var payload = new NetworkPayload
        {
            [DeviceNetworkConstants.Command] = AirAlarmSetMode,
            [AirAlarmSetMode] = mode
        };

        _deviceNet.QueuePacket(uid, null, payload);
    }

    #endregion

    #region Events

    public override void Initialize()
    {
        SubscribeLocalEvent<AirAlarmComponent, DeviceNetworkPacketEvent>(OnPacketRecv);
        SubscribeLocalEvent<AirAlarmComponent, AtmosDeviceUpdateEvent>(OnAtmosUpdate);
        SubscribeLocalEvent<AirAlarmComponent, AtmosAlarmEvent>(OnAtmosAlarm);
        SubscribeLocalEvent<AirAlarmComponent, PowerChangedEvent>(OnPowerChanged);
        SubscribeLocalEvent<AirAlarmComponent, DeviceListUpdateEvent>(OnDeviceListUpdate);
        SubscribeLocalEvent<AirAlarmComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<AirAlarmComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<AirAlarmComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<AirAlarmComponent, ActivateInWorldEvent>(OnActivate);

        Subs.BuiEvents<AirAlarmComponent>(SharedAirAlarmInterfaceKey.Key, subs =>
        {
            subs.Event<BoundUIClosedEvent>(OnClose);
            subs.Event<AirAlarmResyncAllDevicesMessage>(OnResyncAll);
            subs.Event<AirAlarmUpdateAlarmModeMessage>(OnUpdateAlarmMode);
            subs.Event<AirAlarmUpdateAutoModeMessage>(OnUpdateAutoMode);
            subs.Event<AirAlarmUpdateAlarmThresholdMessage>(OnUpdateThreshold);
            subs.Event<AirAlarmUpdateDeviceDataMessage>(OnUpdateDeviceData);
            subs.Event<AirAlarmCopyDeviceDataMessage>(OnCopyDeviceData);
        });
    }

    private void OnDeviceListUpdate(EntityUid uid, AirAlarmComponent component, DeviceListUpdateEvent args)
    {
        var query = GetEntityQuery<DeviceNetworkComponent>();
        foreach (var device in args.OldDevices)
        {
            if (!query.TryGetComponent(device, out var deviceNet))
            {
                continue;
            }

            _atmosDevNet.Deregister(uid, deviceNet.Address);
        }

        component.ScrubberData.Clear();
        component.SensorData.Clear();
        component.VentData.Clear();
        component.KnownDevices.Clear();

        UpdateUI(uid, component);

        SyncRegisterAllDevices(uid);
    }

    private void OnPowerChanged(EntityUid uid, AirAlarmComponent component, ref PowerChangedEvent args)
    {
        if (args.Powered)
        {
            return;
        }

        ForceCloseAllInterfaces(uid);
        component.CurrentModeUpdater = null;
        component.KnownDevices.Clear();
        component.ScrubberData.Clear();
        component.SensorData.Clear();
        component.VentData.Clear();
    }

    private void OnClose(EntityUid uid, AirAlarmComponent component, BoundUIClosedEvent args)
    {
        if (!_ui.IsUiOpen(uid, SharedAirAlarmInterfaceKey.Key))
            RemoveActiveInterface(uid);
    }

    private void OnInit(EntityUid uid, AirAlarmComponent comp, ComponentInit args)
    {
        _deviceLink.EnsureSourcePorts(uid, comp.DangerPort, comp.WarningPort, comp.NormalPort);
    }

    private void OnMapInit(EntityUid uid, AirAlarmComponent comp, MapInitEvent args)
    {
        // for mapped linked air alarms, start with high so when it changes for the first time it goes from high to low
        // without this the output would suddenly get sent a low signal after nothing which is bad
        _deviceLink.SendSignal(uid, GetPort(comp), true);
    }

    private void OnShutdown(EntityUid uid, AirAlarmComponent component, ComponentShutdown args)
    {
        _activeUserInterfaces.Remove(uid);
    }

    private void OnActivate(EntityUid uid, AirAlarmComponent component, ActivateInWorldEvent args)
    {
        if (!args.Complex)
            return;

        if (TryComp<WiresPanelComponent>(uid, out var panel) && panel.Open)
        {
            args.Handled = false;
            return;
        }

        if (!this.IsPowered(uid, EntityManager))
            return;

        _ui.OpenUi(uid, SharedAirAlarmInterfaceKey.Key, args.User);
        AddActiveInterface(uid);
        SyncAllDevices(uid);
        UpdateUI(uid, component);
    }

    private void OnResyncAll(EntityUid uid, AirAlarmComponent component, AirAlarmResyncAllDevicesMessage args)
    {
        if (!AccessCheck(uid, args.Actor, component))
        {
            return;
        }

        component.KnownDevices.Clear();
        component.VentData.Clear();
        component.ScrubberData.Clear();
        component.SensorData.Clear();

        SyncRegisterAllDevices(uid);
    }

    private void OnUpdateAlarmMode(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateAlarmModeMessage args)
    {
        if (AccessCheck(uid, args.Actor, component))
        {
            var addr = string.Empty;
            if (TryComp<DeviceNetworkComponent>(uid, out var netConn))
            {
                addr = netConn.Address;
            }

            _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} changed {ToPrettyString(uid)} mode to {args.Mode}");
            SetMode(uid, addr, args.Mode, false);
        }
        else
        {
            UpdateUI(uid, component);
        }
    }

    private void OnUpdateAutoMode(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateAutoModeMessage args)
    {
        component.AutoMode = args.Enabled;

        _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} changed {ToPrettyString(uid)} auto mode to {args.Enabled}");
        UpdateUI(uid, component);
    }

    private void OnUpdateThreshold(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateAlarmThresholdMessage args)
    {
        if (AccessCheck(uid, args.Actor, component))
        {
            if (args.Gas != null)
                _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} changed {args.Address} {args.Gas} {args.Type} threshold using {ToPrettyString(uid)}");
            else
                _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} changed {args.Address} {args.Type} threshold using {ToPrettyString(uid)}");

            SetThreshold(uid, args.Address, args.Type, args.Threshold, args.Gas);
        }
        else
        {
            UpdateUI(uid, component);
        }
    }

    private void OnUpdateDeviceData(EntityUid uid, AirAlarmComponent component, AirAlarmUpdateDeviceDataMessage args)
    {
        if (AccessCheck(uid, args.Actor, component)
            && _deviceList.ExistsInDeviceList(uid, args.Address))
        {
            _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} changed {args.Address} settings using {ToPrettyString(uid)}");

            SetDeviceData(uid, args.Address, args.Data);
        }
        else
        {
            UpdateUI(uid, component);
        }
    }

    private void OnCopyDeviceData(EntityUid uid, AirAlarmComponent component, AirAlarmCopyDeviceDataMessage args)
    {
        if (!AccessCheck(uid, args.Actor, component))
        {
           UpdateUI(uid, component);
            return;
        }

        switch (args.Data)
        {
            case GasVentPumpData ventData:
                foreach (string addr in component.VentData.Keys)
                {
                    _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} copied settings to vent {addr}");
                    SetData(uid, addr, args.Data);
                }
                break;

            case GasVentScrubberData scrubberData:
                foreach (string addr in component.ScrubberData.Keys)
                {
                    _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Medium, $"{ToPrettyString(args.Actor)} copied settings to scrubber {addr}");
                    SetData(uid, addr, args.Data);
                }
                break;

            case AtmosSensorData sensorData:
                foreach (string addr in component.SensorData.Keys)
                {
                    SetAllThresholds(uid, addr, sensorData);
                }
                break;
        }
    }

    private bool AccessCheck(EntityUid uid, EntityUid? user, AirAlarmComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        // if it has no access reader behave as if the user has AA
        if (!TryComp<AccessReaderComponent>(uid, out var reader))
            return true;

        if (user == null)
            return false;

        if (!_access.IsAllowed(user.Value, uid, reader))
        {
            _popup.PopupEntity(Loc.GetString("air-alarm-ui-access-denied"), user.Value, user.Value);
            _adminLogger.Add(LogType.AtmosDeviceSetting, LogImpact.Low, $"{ToPrettyString(user)} attempted to access {ToPrettyString(uid)} without access");
            return false;
        }

        return true;
    }

    private void OnAtmosAlarm(EntityUid uid, AirAlarmComponent component, AtmosAlarmEvent args)
    {
        if (_ui.IsUiOpen(uid, SharedAirAlarmInterfaceKey.Key))
        {
            SyncAllDevices(uid);
        }

        var addr = string.Empty;
        if (TryComp<DeviceNetworkComponent>(uid, out var netConn))
        {
            addr = netConn.Address;
        }

        if (component.AutoMode)
        {
            if (args.AlarmType == AtmosAlarmType.Danger)
            {
                SetMode(uid, addr, AirAlarmMode.WideFiltering, false);
            }
            else if (args.AlarmType == AtmosAlarmType.Normal || args.AlarmType == AtmosAlarmType.Warning)
            {
                SetMode(uid, addr, AirAlarmMode.Filtering, false);
            }
        }

        if (component.State != args.AlarmType)
        {
            TryComp<DeviceLinkSourceComponent>(uid, out var source);

            // send low to old state's port
            _deviceLink.SendSignal(uid, GetPort(component), false, source);

            // send high to new state's port, along with updating the cached state
            component.State = args.AlarmType;
            _deviceLink.SendSignal(uid, GetPort(component), true, source);
        }

        UpdateUI(uid, component);
    }

    private string GetPort(AirAlarmComponent comp)
    {
        if (comp.State == AtmosAlarmType.Danger)
            return comp.DangerPort;

        if (comp.State == AtmosAlarmType.Warning)
            return comp.WarningPort;

        return comp.NormalPort;
    }

    #endregion

    #region Air Alarm Settings

    /// <summary>
    ///     Set an air alarm's mode.
    /// </summary>
    /// <param name="origin">The origin address of this mode set. Used for network sync.</param>
    /// <param name="mode">The mode to set the alarm to.</param>
    /// <param name="uiOnly">Whether this change is for the UI only, or if it changes the air alarm's operating mode. Defaults to true.</param>
    public void SetMode(EntityUid uid, string origin, AirAlarmMode mode, bool uiOnly = true, AirAlarmComponent? controller = null)
    {
        if (!Resolve(uid, ref controller))
        {
            return;
        }

        if (controller.PanicWireCut)
        {
            mode = AirAlarmMode.Panic;
        }


        controller.CurrentMode = mode;

        // setting it to UI only means we don't have
        // to deal with the issue of not-single-owner
        // alarm mode executors
        if (!uiOnly)
        {
            var newMode = AirAlarmModeFactory.ModeToExecutor(mode);
            if (newMode != null)
            {
                newMode.Execute(uid);
                if (newMode is IAirAlarmModeUpdate updatedMode)
                {
                    controller.CurrentModeUpdater = updatedMode;
                    controller.CurrentModeUpdater.NetOwner = origin;
                }
                else if (controller.CurrentModeUpdater != null)
                    controller.CurrentModeUpdater = null;
            }
        }
        // only one air alarm in a network can use an air alarm mode
        // that updates, so even if it's a ui-only change,
        // we have to invalidate the last mode's updater and
        // remove it because otherwise it'll execute a now
        // invalid mode
        else if (controller.CurrentModeUpdater != null
                 && controller.CurrentModeUpdater.NetOwner != origin)
        {
            controller.CurrentModeUpdater = null;
        }

        UpdateUI(uid, controller);

        // setting sync deals with the issue of air alarms
        // in the same network needing to have the same mode
        // as other alarms
        SyncMode(uid, mode);
    }

    /// <summary>
    ///     Sets device data. Practically a wrapper around the packet sending function, SetData.
    /// </summary>
    /// <param name="address">The address to send the new data to.</param>
    /// <param name="devData">The device data to be sent.</param>
    private void SetDeviceData(EntityUid uid, string address, IAtmosDeviceData devData, AirAlarmComponent? controller = null)
    {
        if (!Resolve(uid, ref controller))
        {
            return;
        }

        devData.Dirty = true;
        SetData(uid, address, devData);
    }

    private void OnPacketRecv(EntityUid uid, AirAlarmComponent controller, DeviceNetworkPacketEvent args)
    {
        if (!args.Data.TryGetValue(DeviceNetworkConstants.Command, out string? cmd))
            return;

        switch (cmd)
        {
            case AtmosDeviceNetworkSystem.SyncData:
                if (!args.Data.TryGetValue(AtmosDeviceNetworkSystem.SyncData, out IAtmosDeviceData? data)
                    || !controller.CanSync)
                    break;

                // Save into component.
                // Sync data to interface.
                switch (data)
                {
                    case GasVentPumpData ventData:
                        if (!controller.VentData.TryAdd(args.SenderAddress, ventData))
                            controller.VentData[args.SenderAddress] = ventData;
                        break;
                    case GasVentScrubberData scrubberData:
                        if (!controller.ScrubberData.TryAdd(args.SenderAddress, scrubberData))
                            controller.ScrubberData[args.SenderAddress] = scrubberData;
                        break;
                    case AtmosSensorData sensorData:
                        if (!controller.SensorData.TryAdd(args.SenderAddress, sensorData))
                            controller.SensorData[args.SenderAddress] = sensorData;
                        break;
                }

                controller.KnownDevices.Add(args.SenderAddress);

                UpdateUI(uid, controller);

                return;
            case AirAlarmSetMode:
                if (!args.Data.TryGetValue(AirAlarmSetMode, out AirAlarmMode alarmMode))
                    break;

                SetMode(uid, args.SenderAddress, alarmMode, uiOnly: false);

                return;
        }
    }

    #endregion

    #region UI

    // List of active user interfaces.
    private readonly HashSet<EntityUid> _activeUserInterfaces = new();

    /// <summary>
    ///     Adds an active interface to be updated.
    /// </summary>
    private void AddActiveInterface(EntityUid uid)
    {
        _activeUserInterfaces.Add(uid);
    }

    /// <summary>
    ///     Removes an active interface from the system update loop.
    /// </summary>
    private void RemoveActiveInterface(EntityUid uid)
    {
        _activeUserInterfaces.Remove(uid);
    }

    /// <summary>
    ///     Force closes all interfaces currently open related to this air alarm.
    /// </summary>
    private void ForceCloseAllInterfaces(EntityUid uid)
    {
        _ui.CloseUi(uid, SharedAirAlarmInterfaceKey.Key);
    }

    private void OnAtmosUpdate(EntityUid uid, AirAlarmComponent alarm, ref AtmosDeviceUpdateEvent args)
    {
        alarm.CurrentModeUpdater?.Update(uid);
    }

    public float CalculatePressureAverage(AirAlarmComponent alarm)
    {
        return alarm.SensorData.Count != 0
            ? alarm.SensorData.Values.Select(v => v.Pressure).Average()
            : 0f;
    }

    public float CalculateTemperatureAverage(AirAlarmComponent alarm)
    {
        return alarm.SensorData.Count != 0
            ? alarm.SensorData.Values.Select(v => v.Temperature).Average()
            : 0f;
    }
    public float CalculateGasMolarConcentrationAverage(AirAlarmComponent alarm, Gas gas, out float percentage)
    {
        percentage = 0f;

        var data = alarm.SensorData.Values.SelectMany(v => v.Gases.Where(g => g.Key == gas));

        if (data.Count() == 0)
            return 0f;

        var averageMol = data.Select(kvp => kvp.Value).Average();
        percentage = data.Select(kvp => kvp.Value).Sum() / alarm.SensorData.Values.Select(v => v.TotalMoles).Sum();

        return averageMol;
    }

    public void UpdateUI(EntityUid uid, AirAlarmComponent? alarm = null, DeviceNetworkComponent? devNet = null, AtmosAlarmableComponent? alarmable = null)
    {
        if (!Resolve(uid, ref alarm, ref devNet, ref alarmable))
        {
            return;
        }

        var pressure = CalculatePressureAverage(alarm);
        var temperature = CalculateTemperatureAverage(alarm);
        var dataToSend = new List<(string, IAtmosDeviceData)>();

        foreach (var (addr, data) in alarm.VentData)
        {
            dataToSend.Add((addr, data));
        }
        foreach (var (addr, data) in alarm.ScrubberData)
        {
            data.AirAlarmPanicWireCut = alarm.PanicWireCut;
            dataToSend.Add((addr, data));
        }
        foreach (var (addr, data) in alarm.SensorData)
        {
            dataToSend.Add((addr, data));
        }

        var deviceCount = alarm.KnownDevices.Count;

        if (!_atmosAlarmable.TryGetHighestAlert(uid, out var highestAlarm))
        {
            highestAlarm = AtmosAlarmType.Normal;
        }

        _ui.SetUiState(
            uid,
            SharedAirAlarmInterfaceKey.Key,
            new AirAlarmUIState(devNet.Address, deviceCount, pressure, temperature, dataToSend, alarm.CurrentMode, highestAlarm.Value, alarm.AutoMode, alarm.PanicWireCut));
    }

    private const float Delay = 8f;
    private float _timer;

    public override void Update(float frameTime)
    {
        _timer += frameTime;
        if (_timer >= Delay)
        {
            _timer = 0f;
            foreach (var uid in _activeUserInterfaces)
            {
                SyncAllSensors(uid);
            }
        }
    }

    #endregion
}
