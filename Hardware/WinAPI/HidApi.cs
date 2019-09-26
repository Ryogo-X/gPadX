using System;
using System.Runtime.InteropServices;

namespace gPadX.Hardware.WinAPI {
    static class HidApi {
        const string DllName = "hid.dll";

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void HidD_GetHidGuid(ref Guid hidGuid);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_GetProductString(IntPtr hidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_GetManufacturerString(IntPtr hidDeviceObject, ref byte lpReportBuffer, int ReportBufferLength);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_GetSerialNumberString(IntPtr hidDeviceObject, ref byte lpReportBuffer, int reportBufferLength);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_GetAttributes(IntPtr hidDeviceObject, ref HIDD_ATTRIBUTES attributes);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_GetCaps(IntPtr preparsedData, ref HIDP_CAPS capabilities);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_GetButtonCaps(HIDP_REPORT_TYPE reportType, [In, Out] HIDP_BUTTON_CAPS[] buttonCaps, ref short buttonCapsLength, IntPtr preparsedData);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_GetValueCaps(HIDP_REPORT_TYPE reportType, [In, Out] HIDP_VALUE_CAPS[] valueCaps, ref short valueCapsLength, IntPtr preparsedData);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_MaxUsageListLength(HIDP_REPORT_TYPE reportType, ushort usagePage, IntPtr preparsedData);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_GetUsages(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, [In, Out] ushort[] usageList, ref int usageLength, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray)] byte[] report, int reportLength);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int HidP_GetUsageValue(HIDP_REPORT_TYPE reportType, ushort usagePage, ushort linkCollection, ushort usage, ref int usageValue, IntPtr preparsedData, [MarshalAs(UnmanagedType.LPArray)] byte[] report, int reportLength);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_GetPreparsedData(IntPtr hidDeviceObject, ref IntPtr preparsedData);

        [DllImport(DllName, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public short VersionNumber;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS {
            public ushort Usage;
            public ushort UsagePage;
            public ushort InputReportByteLength;
            public ushort OutputReportByteLength;
            public ushort FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public ushort[] Reserved;
            public ushort NumberLinkCollectionNodes;
            public ushort NumberInputButtonCaps;
            public ushort NumberInputValueCaps;
            public ushort NumberInputDataIndices;
            public ushort NumberOutputButtonCaps;
            public ushort NumberOutputValueCaps;
            public ushort NumberOutputDataIndices;
            public ushort NumberFeatureButtonCaps;
            public ushort NumberFeatureValueCaps;
            public ushort NumberFeatureDataIndices;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HIDP_BUTTON_CAPS {
            [FieldOffset(0)]
            public ushort UsagePage;
            [FieldOffset(2)]
            public byte ReportID;
            [FieldOffset(3)]
            public bool IsAlias;
            [FieldOffset(4)]
            public ushort BitField;
            [FieldOffset(6)]
            public ushort LinkCollection;
            [FieldOffset(8)]
            public ushort LinkUsage;
            [FieldOffset(10)]
            public ushort LinkUsagePage;
            [FieldOffset(12)]
            public bool IsRange;
            [FieldOffset(13)]
            public bool IsStringRange;
            [FieldOffset(14)]
            public bool IsDesignatorRange;
            [FieldOffset(15)]
            public bool IsAbsolute;
            [FieldOffset(16), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public uint[] Reserved;

            [FieldOffset(56)]
            public HIDP_RANGE Range;
            [FieldOffset(56)]
            public HIDP_NOT_RANGE NotRange;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_RANGE {
            public ushort UsageMin;
            public ushort UsageMax;
            public ushort StringMin;
            public ushort StringMax;
            public ushort DesignatorMin;
            public ushort DesignatorMax;
            public ushort DataIndexMin;
            public ushort DataIndexMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_NOT_RANGE {
            public ushort Usage;
            public ushort Reserved1;
            public ushort StringIndex;
            public ushort Reserved2;
            public ushort DesignatorIndex;
            public ushort Reserved3;
            public ushort DataIndex;
            public ushort Reserved4;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct HIDP_VALUE_CAPS {
            [FieldOffset(0)]
            public ushort UsagePage;
            [FieldOffset(2)]
            public byte ReportID;
            [FieldOffset(3)]
            public bool IsAlias;
            [FieldOffset(4)]
            public ushort BitField;
            [FieldOffset(6)]
            public ushort LinkCollection;
            [FieldOffset(8)]
            public ushort LinkUsage;
            [FieldOffset(10)]
            public ushort LinkUsagePage;
            [FieldOffset(12)]
            public bool IsRange;
            [FieldOffset(13)]
            public bool IsStringRange;
            [FieldOffset(14)]
            public bool IsDesignatorRange;
            [FieldOffset(15)]
            public bool IsAbsolute;
            [FieldOffset(16)]
            public bool HasNull;
            [FieldOffset(17)]
            public byte Reserved;
            [FieldOffset(18)]
            public ushort BitSize;
            [FieldOffset(20)]
            public ushort ReportCount;
            [FieldOffset(22)]
            public ushort Reserved2a;
            [FieldOffset(24)]
            public ushort Reserved2b;
            [FieldOffset(26)]
            public ushort Reserved2c;
            [FieldOffset(28)]
            public ushort Reserved2d;
            [FieldOffset(30)]
            public ushort Reserved2e;
            [FieldOffset(32)]
            public ushort UnitsExp;
            [FieldOffset(36)]
            public ushort Units;
            [FieldOffset(40)]
            public short LogicalMin;
            [FieldOffset(44)]
            public short LogicalMax;
            [FieldOffset(48)]
            public short PhysicalMin;
            [FieldOffset(52)]
            public short PhysicalMax;

            [FieldOffset(56)]
            public HIDP_RANGE Range;
            [FieldOffset(56)]
            public HIDP_NOT_RANGE NotRange;
        }

        public enum HIDP_REPORT_TYPE : ushort {
            Input,
            Output,
            Feature,
            Count // for arrays
        }
    }
}
