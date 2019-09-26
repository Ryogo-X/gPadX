using System;
using System.Runtime.InteropServices;

namespace gPadX.Hardware.WinAPI {
    static class Kernel32Api {
        const string DllName = "kernel32.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CancelIoEx(IntPtr hFile, IntPtr lpOverlapped);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;

            public static SECURITY_ATTRIBUTES Create() {
                return new SECURITY_ATTRIBUTES() {
                    bInheritHandle = true,
                    nLength = Marshal.SizeOf(typeof(SECURITY_ATTRIBUTES))
                };
            }
        }
    }
}