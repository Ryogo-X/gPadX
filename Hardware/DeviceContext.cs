using System;
using gPadX.Hardware.WinAPI;

namespace gPadX.Hardware {
    class DeviceContext : IDisposable {
        public IntPtr Handle { get; }

        public DeviceModeType DeviceMode { get; }
        public AccessModeType AccessMode { get; }
        public ShareModeType ShareMode { get; }


        public DeviceContext(string devicePath, DeviceModeType deviceMode = DeviceModeType.NonOverlapped, AccessModeType accessMode = AccessModeType.None, ShareModeType shareMode = ShareModeType.ShareRead | ShareModeType.ShareWrite) {
            Handle = OpenDevice(devicePath, deviceMode, accessMode, shareMode);
        }

        IntPtr OpenDevice(string devicePath, DeviceModeType deviceMode, AccessModeType accessMode, ShareModeType shareMode) {
            var securityAttributes = Kernel32Api.SECURITY_ATTRIBUTES.Create();
            var flags = 0;

            // FILE_FLAG_OVERLAPPED is used for async i/o
            //if (deviceMode == DeviceModeType.Overlapped) {
                flags = 0x40000000; //FILE_FLAG_OVERLAPPED;
            //}

            // internal const short FILE_SHARE_READ = 0x1;
            // internal const short FILE_SHARE_WRITE = 0x2;
            // may be required to set shareMode
            //dwCreationDisposition == 3 (OPEN_EXISTING)
            return Kernel32Api.CreateFile(devicePath, (uint)accessMode, (int)shareMode, ref securityAttributes, 3, flags, 0);
        }

        void CloseDevice(IntPtr handle) {
            if (Environment.OSVersion.Version.Major > 5) {
                Kernel32Api.CancelIoEx(handle, IntPtr.Zero);
            }

            Kernel32Api.CloseHandle(handle);
        }

        public void Dispose() {
            if (Handle == IntPtr.Zero) { return; }

            CloseDevice(Handle);
        }

        public enum DeviceModeType {
            NonOverlapped = 0,
            Overlapped = 1
        }

        [Flags]
        public enum ShareModeType : short {
            Exclusive = 0,
            ShareRead = 1, //NativeMethods.FILE_SHARE_READ,
            ShareWrite = 2 //NativeMethods.FILE_SHARE_WRITE
        }

        public enum AccessModeType : uint {
            None = 0,
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000
        }
    }
}
