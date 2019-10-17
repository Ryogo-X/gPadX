using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using gPadX.Hardware;
using gPadX.Utility;
using gPadX.Views;
using gPadX.WPF;

namespace gPadX.ViewModels {
    class MainViewModel : ViewModelBase {
        public ObservableCollection<DeviceViewModel> Devices { get; } = new ObservableCollection<DeviceViewModel>();
        public ICommand RefreshCommand { get; }
        public ICommand ViewDevicesCommand { get; }

        bool isRefreshing;
        public bool IsRefreshing {
            get { return isRefreshing; }
            set { Set(ref isRefreshing, value); }
        }

        public MainViewModel() {
            RefreshCommand = new RelayCommand(OnRefresh);
            ViewDevicesCommand = new RelayCommand(OnViewDevices);

            OnRefresh();
        }

        void OnRefresh(object arg = null) {
            IsRefreshing = true;

            Task.Run(() => {
                var devices = DeviceManager.GetDevices(DeviceManager.DeviceType.Joystick, DeviceManager.DeviceType.Gamepad);

                // removing disconnected devices
                var devicesToRemove = Devices.Where(x => !devices.Any(y => y.Id == x.Id)).ToArray();
                foreach (var device in devicesToRemove) {
                    Application.Current.Dispatcher.Invoke(() => {
                        Devices.Remove(device);
                    });
                }

                // adding connected devices 
                foreach (var device in devices) {
                    var config = ConfigManager.Config.DeviceConfigs?.FirstOrDefault(x => x.Id == device.Id);
                    if (config == null || Devices.Any(x => x.Id == device.Id)) { continue; }

                    Application.Current.Dispatcher.Invoke(() => {
                        Devices.Add(new DeviceViewModel(config));
                    });
                }
            }).ContinueWith(_ => {
                IsRefreshing = false;
            });
        }

        void OnViewDevices(object arg) {
            var vm = new DevicesViewModel(Devices);
            var view = new DevicesView();
            view.DataContext = vm;
            if (view.ShowDialog() != true) { return; }

            Devices.Clear();
            foreach(var device in vm.Devices.Where(x => x.IsSelected)) {
                Devices.Add(device);
            }

            ConfigManager.Config.DeviceConfigs = Devices.Select(x => x.GetConfig()).ToArray();
        }
    }
}
