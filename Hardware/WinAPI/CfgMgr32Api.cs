using System.Runtime.InteropServices;

namespace gPadX.Hardware.WinAPI {
    static class CfgMgr32Api {
        const string DllName = "cfgmgr32.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int CM_Get_Device_ID_Size(out int length, uint devInst, int flags = 0);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int CM_Get_Device_ID(uint devInst, char[] buffer, int length, int flags = 0);
    }
}
