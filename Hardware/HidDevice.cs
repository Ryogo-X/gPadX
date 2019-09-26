using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using gPadX.Hardware.WinAPI;
using Microsoft.Win32.SafeHandles;

namespace gPadX.Hardware {
    class HidDevice {
        protected HidDeviceCapabilities capabilities;

        public string Path { get; }
        public string Id { get; }
        public string Vendor { get; }
        public string Product { get; }
        //TODO: convert VendorId / ProductId to hex-strings
        public ushort VendorId { get; }
        public ushort ProductId { get; }

        public ushort UsagePage {
            get { return capabilities.UsagePage; }
        }

        public ushort Usage {
            get { return capabilities.Usage; }
        }

        public HidDevice(string devicePath, string id) {
            Path = devicePath;
            Id = id;

            using (var device = new DeviceContext(Path)) {
                Vendor = GetVendor(device.Handle);
                Product = GetProduct(device.Handle);
                var attributes = GetAttributes(device.Handle);
                VendorId = attributes.VendorId;
                ProductId = attributes.ProductId;
                capabilities = GetDeviceCapabilities(device.Handle);
            }
        }

        public HidDeviceReport GetReport() {
            var buffer = ArrayPool<byte>.Shared.Rent(capabilities.InputReportByteLength);
            var status = HidDeviceReport.ReadStatus.NoDataRead;

            try {
                using (var device = new DeviceContext(Path, accessMode: DeviceContext.AccessModeType.GenericRead | DeviceContext.AccessModeType.GenericWrite)) {
                    using (var fileHandle = new SafeFileHandle(device.Handle, false)) {
                        using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read, buffer.Length)) {
                            fs.Read(buffer, 0, buffer.Length);
                        }
                    }

                    var newBuffer = new byte[capabilities.InputReportByteLength];
                    Array.Copy(buffer, 0, newBuffer, 0, newBuffer.Length);

                    return new HidDeviceReport(newBuffer, HidDeviceReport.ReadStatus.Success);
                }
            } catch {
                status = HidDeviceReport.ReadStatus.ReadError;
            } finally {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return new HidDeviceReport(status);
        }

        public async Task<HidDeviceReport> GetReportAsync(CancellationToken token) {
            var buffer = ArrayPool<byte>.Shared.Rent(capabilities.InputReportByteLength);
            var status = HidDeviceReport.ReadStatus.NoDataRead;

            try {
                using (var device = new DeviceContext(Path, accessMode: DeviceContext.AccessModeType.GenericRead | DeviceContext.AccessModeType.GenericWrite)) {
                    using (var fileHandle = new SafeFileHandle(device.Handle, false)) {
                        using (var fs = new System.IO.FileStream(fileHandle, System.IO.FileAccess.Read, buffer.Length, true)) {
                            await fs.ReadAsync(buffer, 0, buffer.Length, token);
                        }
                    }

                    var newBuffer = new byte[capabilities.InputReportByteLength];
                    Array.Copy(buffer, 0, newBuffer, 0, newBuffer.Length);

                    return new HidDeviceReport(newBuffer, HidDeviceReport.ReadStatus.Success);
                }
            } catch {
                status = HidDeviceReport.ReadStatus.ReadError;
            } finally {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return new HidDeviceReport(status);
        }

        #region Initialization
        string GetVendor(IntPtr handle) {
            var data = new byte[1024];
            HidApi.HidD_GetManufacturerString(handle, ref data[0], data.Length);

            return Encoding.Unicode.GetString(data).Trim('\0');
        }

        string GetProduct(IntPtr handle) {
            var data = new byte[1024];
            HidApi.HidD_GetProductString(handle, ref data[0], data.Length);

            return Encoding.Unicode.GetString(data).Trim('\0');
        }

        (ushort VendorId, ushort ProductId) GetAttributes(IntPtr handle) {
            var deviceAttributes = default(HidApi.HIDD_ATTRIBUTES);
            deviceAttributes.Size = Marshal.SizeOf(deviceAttributes);
            HidApi.HidD_GetAttributes(handle, ref deviceAttributes);

            return (VendorId: deviceAttributes.VendorID, ProductId: deviceAttributes.ProductID);
        }

        public static HidDeviceCapabilities GetDeviceCapabilities(IntPtr handle) {
            var capabilities = default(HidApi.HIDP_CAPS);

            using (var context = new PreparsedDataContext(handle)) {
                HidApi.HidP_GetCaps(context.Handle, ref capabilities);
            }

            return new HidDeviceCapabilities(capabilities);
        }
        #endregion

        public class HidDeviceReport {
            public byte[] Data { get; }
            public ReadStatus Status { get; }

            public HidDeviceReport(ReadStatus status) {
                Data = new byte[] { };
                Status = status;
            }

            public HidDeviceReport(byte[] data, ReadStatus status) {
                Data = data;
                Status = status;
            }

            public enum ReadStatus {
                Success = 0,
                WaitTimedOut = 1,
                WaitFail = 2,
                NoDataRead = 3,
                ReadError = 4,
                NotConnected = 5
            }
        }

        public class HidDeviceCapabilities {
            public HidDeviceCapabilities(HidApi.HIDP_CAPS caps) {
                Usage = caps.Usage;
                UsagePage = caps.UsagePage;
                InputReportByteLength = caps.InputReportByteLength;
                OutputReportByteLength = caps.OutputReportByteLength;
                FeatureReportByteLength = caps.FeatureReportByteLength;
                Reserved = caps.Reserved;
                NumberLinkCollectionNodes = caps.NumberLinkCollectionNodes;
                NumberInputButtonCaps = caps.NumberInputButtonCaps;
                NumberInputValueCaps = caps.NumberInputValueCaps;
                NumberInputDataIndices = caps.NumberInputDataIndices;
                NumberOutputButtonCaps = caps.NumberOutputButtonCaps;
                NumberOutputValueCaps = caps.NumberOutputValueCaps;
                NumberOutputDataIndices = caps.NumberOutputDataIndices;
                NumberFeatureButtonCaps = caps.NumberFeatureButtonCaps;
                NumberFeatureValueCaps = caps.NumberFeatureValueCaps;
                NumberFeatureDataIndices = caps.NumberFeatureDataIndices;
            }

            public ushort Usage { get; private set; }
            public ushort UsagePage { get; private set; }
            public ushort InputReportByteLength { get; private set; }
            public ushort OutputReportByteLength { get; private set; }
            public ushort FeatureReportByteLength { get; private set; }
            public ushort[] Reserved { get; private set; }
            public ushort NumberLinkCollectionNodes { get; private set; }
            public ushort NumberInputButtonCaps { get; private set; }
            public ushort NumberInputValueCaps { get; private set; }
            public ushort NumberInputDataIndices { get; private set; }
            public ushort NumberOutputButtonCaps { get; private set; }
            public ushort NumberOutputValueCaps { get; private set; }
            public ushort NumberOutputDataIndices { get; private set; }
            public ushort NumberFeatureButtonCaps { get; private set; }
            public ushort NumberFeatureValueCaps { get; private set; }
            public ushort NumberFeatureDataIndices { get; private set; }
        }
    }
}
