using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gPadX.Hardware.WinAPI;

namespace gPadX.Hardware {
    class GamepadDevice : HidDevice {
        readonly HidDeviceButtonCapabilities[] buttonCapabilities;
        readonly HidDeviceValueCapabilities[] valueCapabilities;

        public GamepadDevice(string devicePath, string id) : base(devicePath, id) {
            using (var deviceContext = new DeviceContext(devicePath)) {
                buttonCapabilities = GetDeviceButtonCapabilities(deviceContext.Handle);
                valueCapabilities = GetDeviceValueCapabilities(deviceContext.Handle);
            }
        }

        public async Task<GamepadState> GetStateAsync(CancellationToken token) {
            var report = await GetReportAsync(token);
            var buttonCaps = buttonCapabilities[0];
            ushort[] buttonsState;

            var state = new GamepadState();
            using (var deviceContext = new DeviceContext(Path)) {
                using (var dataContext = new PreparsedDataContext(deviceContext.Handle)) {
                    var buttonsStateLength = buttonCaps.Range.UsageMax - buttonCaps.Range.UsageMin + 1;
                    //var buttonsStateLength = HidApi.HidP_MaxUsageListLength(HidApi.HIDP_REPORT_TYPE.Input, buttonCaps.UsagePage, dataContext.Handle);
                    buttonsState = new ushort[buttonsStateLength];
                    HidApi.HidP_GetUsages(HidApi.HIDP_REPORT_TYPE.Input, buttonCaps.UsagePage, 0, buttonsState, ref buttonsStateLength, dataContext.Handle, report.Data, report.Data.Length);

                    if (buttonsState.Contains((ushort)2)) {
                        state.A = true;
                    } 
                    if (buttonsState.Contains((ushort)3)) {
                        state.B = true;
                    } 
                    if (buttonsState.Contains((ushort)1)) {
                        state.X = true;
                    } 
                    if (buttonsState.Contains((ushort)4)) {
                        state.Y = true;
                    } 

                    if (buttonsState.Contains((ushort)5)) {
                        state.L1 = true;
                    }
                    if (buttonsState.Contains((ushort)7)) {
                        state.L2 = true;
                    }

                    if (buttonsState.Contains((ushort)6)) {
                        state.R1 = true;
                    }
                    if (buttonsState.Contains((ushort)8)) {
                        state.R2 = true;
                    }

                    if (buttonsState.Contains((ushort)9)) {
                        state.Select = true;
                    }
                    if (buttonsState.Contains((ushort)10)) {
                        state.Start = true;
                    }

                    if (buttonsState.Contains((ushort)11)) {
                        state.L3 = true;
                    }
                    if (buttonsState.Contains((ushort)12)) {
                        state.R3 = true;
                    }

                    var hasDpad = valueCapabilities.Any(x => x.Range.UsageMin == 0x39);
                    for (var i = 0; i < capabilities.NumberInputValueCaps; i++) {
                        int value = 0;
                        HidApi.HidP_GetUsageValue(HidApi.HIDP_REPORT_TYPE.Input, valueCapabilities[i].UsagePage, 0, valueCapabilities[i].Range.UsageMin, ref value, dataContext.Handle, report.Data, report.Data.Length);

                        var scaleValue = 256;
                        if (valueCapabilities[i].UsagePage == 1) {
                            switch (valueCapabilities[i].Range.UsageMin) {
                                case 0x30:  // X-axis // LX
                                    var lAxisX = value - GamepadState.DEFAULT_AXIS_VALUE; // LogMin: 0 / LogMax: 255 // default: 128
                                    if (value == 0) { lAxisX++; }
                                    if (hasDpad) {
                                        state.LX = lAxisX * scaleValue;
                                    } else {
                                        if (valueCapabilities[i].LogicalMin + 1 == value) {
                                            state.Left = true;
                                        }
                                        if (valueCapabilities[i].LogicalMax == value) {
                                            state.Right = true;
                                        }
                                    }
                                    break;

                                case 0x31:  // Y-axis // LY
                                    var lAxisY = GamepadState.DEFAULT_AXIS_VALUE - value; //(long)value - 128; // LogMin: 0 / LogMax: 255 // default: 128
                                    if (value == 0) { lAxisY--; }
                                    if (hasDpad) {
                                        state.LY = lAxisY * scaleValue;
                                    } else {
                                        if (valueCapabilities[i].LogicalMin + 1 == value) {
                                            state.Up = true;
                                        }
                                        if (valueCapabilities[i].LogicalMax == value) {
                                            state.Down = true;
                                        }
                                    }
                                    break;

                                case 0x32: // Z-axis // RX
                                    var lAxisZ = value - GamepadState.DEFAULT_AXIS_VALUE; // LogMin: 0 / LogMax: 255 // default: 128
                                    if (value == 0) { lAxisZ++; }
                                    state.RX = lAxisZ * scaleValue;
                                    break;

                                case 0x35: // Rotate-Z // RY
                                    var lAxisRz = value - GamepadState.DEFAULT_AXIS_VALUE; // LogMin: 0 / LogMax: 255 // default: 128
                                    if (value == 0) { lAxisRz++; }
                                    state.RY = lAxisRz * scaleValue;
                                    break;

                                case 0x39:  // Hat Switch // DPAD 
                                    var lHat = value; // LogMin: 0 / LogMax: 7 // default: 8 // val = 360 / 8
                                    if (lHat == 0) {
                                        state.Up = true;
                                    } else if (lHat == 1) {
                                        state.Up = true;
                                        state.Right = true;
                                    } else if (lHat == 2) {
                                        state.Right = true;
                                    } else if (lHat == 3) {
                                        state.Right = true;
                                        state.Down = true;
                                    } else if (lHat == 4) {
                                        state.Down = true;
                                    } else if (lHat == 5) {
                                        state.Down = true;
                                        state.Left = true;
                                    } else if (lHat == 6) {
                                        state.Left = true;
                                    } else if (lHat == 7) {
                                        state.Left = true;
                                        state.Up = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return state;
        }

        HidDeviceButtonCapabilities[] GetDeviceButtonCapabilities(IntPtr handle) {
            var capsLength = (short)capabilities.NumberInputButtonCaps;
            var buttonCapabilities = new HidApi.HIDP_BUTTON_CAPS[capsLength];

            using (var dataContext = new PreparsedDataContext(handle)) {
                HidApi.HidP_GetButtonCaps(HidApi.HIDP_REPORT_TYPE.Input, buttonCapabilities, ref capsLength, dataContext.Handle);
            }

            return buttonCapabilities.Select(caps => new HidDeviceButtonCapabilities(caps)).ToArray();
        }

        HidDeviceValueCapabilities[] GetDeviceValueCapabilities(IntPtr handle) {
            var capsLength = (short)capabilities.NumberInputValueCaps;
            var valueCapabilities = new HidApi.HIDP_VALUE_CAPS[capsLength];

            using (var dataContext = new PreparsedDataContext(handle)) {
                HidApi.HidP_GetValueCaps(HidApi.HIDP_REPORT_TYPE.Input, valueCapabilities, ref capsLength, dataContext.Handle);
            }

            return valueCapabilities.Select(caps => new HidDeviceValueCapabilities(caps)).ToArray();
        }

        public class HidDeviceButtonCapabilities {
            public ushort UsagePage { get; set; }
            public byte ReportId { get; set; }
            public bool IsAlias { get; set; }
            public ushort BitField { get; set; }
            public ushort LinkCollection { get; set; }
            public ushort LinkUsage { get; set; }
            public ushort LinkUsagePage { get; set; }
            public bool IsRange { get; set; }
            public bool IsStringRange { get; set; }
            public bool IsDesignatorRange { get; set; }
            public bool IsAbsolute { get; set; }
            public uint[] Reserved { get; set; }

            public HidDeviceButtonRange Range { get; set; }
            public HidDeviceButtonNotRange NotRange { get; set; }

            internal HidDeviceButtonCapabilities(HidApi.HIDP_BUTTON_CAPS caps) {
                UsagePage = caps.UsagePage;
                ReportId = caps.ReportID;
                IsAlias = caps.IsAlias;
                BitField = caps.BitField;
                LinkCollection = caps.LinkCollection;
                LinkUsage = caps.LinkUsage;
                LinkUsagePage = caps.LinkUsagePage;
                IsRange = caps.IsRange;
                IsStringRange = caps.IsStringRange;
                IsDesignatorRange = caps.IsDesignatorRange;
                IsAbsolute = caps.IsAbsolute;
                Reserved = caps.Reserved;

                Range = new HidDeviceButtonRange(caps.Range);
                NotRange = new HidDeviceButtonNotRange(caps.NotRange);
            }
        }

        public class HidDeviceValueCapabilities {
            public ushort UsagePage { get; set; }
            public byte ReportId { get; set; }
            public bool IsAlias { get; set; }
            public ushort BitField { get; set; }
            public ushort LinkCollection { get; set; }
            public ushort LinkUsage { get; set; }
            public ushort LinkUsagePage { get; set; }
            public bool IsRange { get; set; }
            public bool IsStringRange { get; set; }
            public bool IsDesignatorRange { get; set; }
            public bool IsAbsolute { get; set; }
            public bool HasNull { get; set; }
            public byte Reserved { get; set; }
            public ushort BitSize { get; set; }
            public ushort ReportCount { get; set; }
            public ushort[] Reserved2 { get; set; }
            public ushort UnitsExp { get; set; }
            public ushort Units { get; set; }
            public short LogicalMin { get; set; }
            public short LogicalMax { get; set; }
            public short PhysicalMin { get; set; }
            public short PhysicalMax { get; set; }

            public HidDeviceButtonRange Range { get; set; }
            public HidDeviceButtonNotRange NotRange { get; set; }

            internal HidDeviceValueCapabilities(HidApi.HIDP_VALUE_CAPS caps) {
                UsagePage = caps.UsagePage;
                ReportId = caps.ReportID;
                IsAlias = caps.IsAlias;
                BitField = caps.BitField;
                LinkCollection = caps.LinkCollection;
                LinkUsage = caps.LinkUsage;
                LinkUsagePage = caps.LinkUsagePage;
                IsRange = caps.IsRange;
                IsStringRange = caps.IsStringRange;
                IsDesignatorRange = caps.IsDesignatorRange;
                IsAbsolute = caps.IsAbsolute;
                HasNull = caps.HasNull;
                Reserved = caps.Reserved;
                BitSize = caps.BitSize;
                ReportCount = caps.ReportCount;
                Reserved2 = new[] {
                    caps.Reserved2a,
                    caps.Reserved2b,
                    caps.Reserved2c,
                    caps.Reserved2d,
                    caps.Reserved2e
                };
                UnitsExp = caps.UnitsExp;
                Units = caps.Units;
                LogicalMin = caps.LogicalMin;
                LogicalMax = caps.LogicalMax;
                PhysicalMin = caps.PhysicalMin;
                PhysicalMax = caps.PhysicalMax;

                Range = new HidDeviceButtonRange(caps.Range);
                NotRange = new HidDeviceButtonNotRange(caps.NotRange);
            }
        }

        public class HidDeviceButtonRange {
            public ushort UsageMin { get; set; }
            public ushort UsageMax { get; set; }
            public ushort StringMin { get; set; }
            public ushort StringMax { get; set; }
            public ushort DesignatorMin { get; set; }
            public ushort DesignatorMax { get; set; }
            public ushort DataIndexMin { get; set; }
            public ushort DataIndexMax { get; set; }

            internal HidDeviceButtonRange(HidApi.HIDP_RANGE range) {
                UsageMin = range.UsageMin;
                UsageMax = range.UsageMax;
                StringMin = range.StringMin;
                StringMax = range.StringMax;
                DesignatorMin = range.DesignatorMin;
                DesignatorMax = range.DesignatorMax;
                DataIndexMin = range.DataIndexMin;
                DataIndexMax = range.DataIndexMax;
            }
        }

        public class HidDeviceButtonNotRange {
            public ushort Usage { get; set; }
            public ushort Reserved1 { get; set; }
            public ushort StringIndex { get; set; }
            public ushort Reserved2 { get; set; }
            public ushort DesignatorIndex { get; set; }
            public ushort Reserved3 { get; set; }
            public ushort DataIndex { get; set; }
            public ushort Reserved4 { get; set; }

            internal HidDeviceButtonNotRange(HidApi.HIDP_NOT_RANGE notRange) {
                Usage = notRange.Usage;
                Reserved1 = notRange.Reserved1;
                StringIndex = notRange.StringIndex;
                Reserved2 = notRange.Reserved2;
                DesignatorIndex = notRange.DesignatorIndex;
                Reserved3 = notRange.Reserved3;
                DataIndex = notRange.DataIndex;
                Reserved4 = notRange.Reserved4;
            }
        }
    }
}
