using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using gPadX.Hardware;
using gPadX.Models;
using gPadX.Views;
using gPadX.WPF;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;

namespace gPadX.ViewModels {
    class DeviceViewModel : ViewModelBase {
        CancellationTokenSource emulationCancellation;

        readonly DeviceConfigModel config;

        public string Path { get; set; }
        public string Id { get; set; }
        public string Vendor { get; set; }
        public string Product { get; set; }

        public string DisplayName {
            get {
                if (!string.IsNullOrWhiteSpace(config.Alias)) {
                    return config.Alias;
                } else if (Product.Contains(Vendor)) {
                    return Product;
                } else {
                    return $"{Vendor} {Product}";
                }
            }
        }

        bool isEmulated;
        public bool IsEmulated {
            get { return isEmulated; }
            private set { Set(ref isEmulated, value); }
        }

        bool isSelected;
        public bool IsSelected {
            get { return isSelected; }
            set { Set(ref isSelected, value); }
        }

        public ICommand StartEmulationCommand { get; }
        public ICommand StopEmulationCommand { get; }
        public ICommand ConfigCommand { get; }

        DeviceViewModel() {
            StartEmulationCommand = new RelayCommand(OnStartEmulation);
            StopEmulationCommand = new RelayCommand(OnStopEmulation);
            ConfigCommand = new RelayCommand<DeviceViewModel>(OnConfigDevice);
        }

        public DeviceViewModel(string deviceId) : this() {
            var device = DeviceManager.GetDevice(deviceId);
            Path = device.Path;
            Id = device.Id;
            Vendor = device.Vendor;
            Product = device.Product;

            if (config == null) {
                config = new DeviceConfigModel();
                config.Id = Id;
            }
        }

        public DeviceViewModel(DeviceConfigModel config) : this(config.Id) {
            this.config = config;
        }

        public DeviceConfigModel GetConfig() {
            return config;
        }

        void OnConfigDevice(DeviceViewModel device) {
            var vm = new DeviceConfigViewModel(device.GetConfig());
            var view = new DeviceConfigView();
            view.DataContext = vm;
            if (view.ShowDialog() != true) { return; }

            Notify(nameof(DisplayName));
        }

        void OnStartEmulation(object arg) {
            var client = new ViGEmClient();

            var controller = client.CreateXbox360Controller();
            controller.AutoSubmitReport = false;
            controller.Connect();

            emulationCancellation = new CancellationTokenSource();
            var token = emulationCancellation.Token;

            Task.Run(async () => {
                var prevState = new GamepadState();

                try {
                    var gamepad = (GamepadDevice)DeviceManager.GetDevice(Path, Id);
                    while (!token.IsCancellationRequested) {
                        var state = await gamepad.GetStateAsync(token);
                        if (token.IsCancellationRequested || prevState == state && state.IsDefault) { continue; }

                        controller.ResetReport();
                        SetReport(controller, state);
                        controller.SubmitReport();
                        prevState = state;
                    }
                } finally {
                    client.Dispose();
                    IsEmulated = false;
                }
            }, token);

            IsEmulated = true;
        }

        void OnStopEmulation(object arg) {
            emulationCancellation?.Cancel();
            emulationCancellation = null;
        }

        void PreprocessState(GamepadState state) {
            if (Math.Abs(state.LX) - (config.LSDeadZone * 256) <= 0) { state.LX = 0; }
            if (Math.Abs(state.LY) - (config.LSDeadZone * 256) <= 0) { state.LY = 0; }
            if (Math.Abs(state.RX) - (config.RSDeadZone * 256) <= 0) { state.RX = 0; }
            if (Math.Abs(state.RY) - (config.RSDeadZone * 256) <= 0) { state.RY = 0; }

            if (config.MapDPadToLS) {
                if (state.Left) {
                    state.Left = false;
                    state.LX = -(127 * 256);
                } else if (state.Right) {
                    state.Right = false;
                    state.LX = (127 * 256);
                }

                if (state.Up) {
                    state.Up = false;
                    state.LY = (127 * 256);
                } else if (state.Down) {
                    state.Down = false;
                    state.LY = -(127 * 256);
                }
            }
        }

        void SetReport(IXbox360Controller controller, GamepadState state) {
            PreprocessState(state);

            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Up, state.Up);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Down, state.Down);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Left, state.Left);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Right, state.Right);

            controller.SetAxisValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Axis.LeftThumbX, (short)state.LX);
            controller.SetAxisValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Axis.LeftThumbY, (short)state.LY);

            controller.SetAxisValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Axis.RightThumbX, (short)state.RX);
            controller.SetAxisValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Axis.RightThumbY, (short)state.RY);

            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.A, state.A);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.B, state.B);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.X, state.X);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Y, state.Y);

            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Back, state.Select);
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.Start, state.Start);

            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.LeftShoulder, state.L1);
            controller.SetSliderValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Slider.LeftTrigger, (byte)(state.L2 ? 255 : 0));
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.LeftThumb, state.L3);

            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.RightShoulder, state.R1);
            controller.SetSliderValue(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Slider.RightTrigger, (byte)(state.R2 ? 255 : 0));
            controller.SetButtonState(Nefarius.ViGEm.Client.Targets.Xbox360.Xbox360Button.RightThumb, state.R3);
        }
    }
}
