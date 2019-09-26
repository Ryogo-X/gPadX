using System;
using gPadX.Hardware.WinAPI;

namespace gPadX.Hardware {
    class PreparsedDataContext : IDisposable {
        public IntPtr Handle { get; }

        public PreparsedDataContext(IntPtr deviceHandle) {
            var handle = IntPtr.Zero;
            if (!HidApi.HidD_GetPreparsedData(deviceHandle, ref handle)) {
                throw new InvalidOperationException("Failed to get PreparsedData pointer");
            }

            Handle = handle;
        }

        public void Dispose() {
            if (Handle == IntPtr.Zero) { return; }

            HidApi.HidD_FreePreparsedData(Handle);
        }
    }
}
