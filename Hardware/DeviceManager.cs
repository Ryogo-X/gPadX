using System;
using System.Collections.Generic;
using System.Linq;
using gPadX.Hardware.WinAPI;

namespace gPadX.Hardware {
    static class DeviceManager {
        const int INVALID_HANDLE_VALUE = -1;

        static Lazy<Guid> hidClassGuid = new Lazy<Guid>(() => {
            Guid guid = default;
            HidApi.HidD_GetHidGuid(ref guid);

            return guid;
        });

        public static IReadOnlyList<HidDevice> GetDevices(params DeviceType[] deviceTypes) {
            var devices = new List<HidDevice>();

            //var devices = new List<DeviceInfo>();
            var hidClass = hidClassGuid.Value;
            var deviceInfoSet = SetupApi.SetupDiGetClassDevs(ref hidClass, null, 0, SetupApi.DIGCF_PRESENT | SetupApi.DIGCF_DEVICEINTERFACE);
            if (deviceInfoSet.ToInt64() == INVALID_HANDLE_VALUE) { throw new InvalidOperationException("Not able to retrieve device list"); }

            var deviceInfoData = SetupApi.SP_DEVINFO_DATA.Create();
            var deviceIndex = 0;

            while (SetupApi.SetupDiEnumDeviceInfo(deviceInfoSet, deviceIndex, ref deviceInfoData)) {
                deviceIndex += 1;

                var deviceInterfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Create();
                var deviceInterfaceIndex = 0;

                while (SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref hidClass, deviceInterfaceIndex, ref deviceInterfaceData)) {
                    deviceInterfaceIndex++;

                    var devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
                    var deviceId = GetDeviceId(deviceInfoData.DevInst);
                    var device = GetDevice(devicePath, deviceId);
                    if (deviceTypes?.Any() == true && !deviceTypes.Contains((DeviceType)device.Usage)) { continue; }

                    devices.Add(device);
                }
            }
            SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);

            return devices;
        }

        public static HidDevice GetDevice(string deviceId) {
            return GetDevices(DeviceType.Joystick, DeviceType.Gamepad).FirstOrDefault(x => x.Id == deviceId);
        }

        public static HidDevice GetDevice(string devicePath, string deviceId) {
            HidDevice.HidDeviceCapabilities capabilities = null;

            using (var device = new DeviceContext(devicePath)) {
                capabilities = HidDevice.GetDeviceCapabilities(device.Handle);
            }

            var usageType = (DeviceType)capabilities.Usage;
            if (usageType == DeviceType.Joystick || usageType == DeviceType.Gamepad) {
                return new GamepadDevice(devicePath, deviceId);
            } else {
                return new HidDevice(devicePath, deviceId);
            }
        }

        static string GetDevicePath(IntPtr deviceInfoSet, SetupApi.SP_DEVICE_INTERFACE_DATA deviceInterfaceData) {
            var bufferSize = 0;
            var interfaceDetail = SetupApi.SP_DEVICE_INTERFACE_DETAIL_DATA.Create();

            SetupApi.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
            return SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref interfaceDetail, bufferSize, ref bufferSize, IntPtr.Zero) ? interfaceDetail.DevicePath : null;
        }

        static string GetDeviceId(uint devInst) {
            if (CfgMgr32Api.CM_Get_Device_ID_Size(out var length, devInst) != 0) {
                throw new InvalidOperationException("Failed to get DeviceId length");
            }

            var data = new char[length + 1];
            if (CfgMgr32Api.CM_Get_Device_ID(devInst, data, data.Length) != 0) {
                throw new InvalidOperationException("Failed to get DeviceId");
            }

            return new string(data);
        }

        public enum DeviceType : short {
            Joystick = 4,
            Gamepad = 5
        }
    }
}