using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using gPadX.Hardware;
using gPadX.WPF;

namespace gPadX.ViewModels {
    class DevicesViewModel : ViewModel {
        public ObservableCollection<DeviceViewModel> Devices { get; } = new ObservableCollection<DeviceViewModel>();
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand DeselectCommand { get; }

        bool isRefreshing;
        public bool IsRefreshing {
            get { return isRefreshing; }
            set { Set(ref isRefreshing, value); }
        }

        public DevicesViewModel(IEnumerable<DeviceViewModel> devices = null) {
            RefreshCommand = new RelayCommand(OnRefresh);

            CancelCommand = new RelayCommand(OnCancel);
            ApplyCommand = new RelayCommand(OnApply);

            DeselectCommand = new RelayCommand<DeviceViewModel>(OnDeselect);
            SelectCommand = new RelayCommand<DeviceViewModel>(OnSelect);

            if (devices != null) {
                foreach (var device in devices) {
                    device.IsSelected = true;
                    Devices.Add(device);
                }
            }

            OnRefresh();
        }

        void OnRefresh(object arg = null) {
            IsRefreshing = true;

            Task.Run(() => {
                var devices = DeviceManager.GetDevices(DeviceManager.DeviceType.Gamepad, DeviceManager.DeviceType.Joystick);
                foreach (var device in devices) {
                    if (Devices.Any(x => x.Id == device.Id)) { continue; }

                    Application.Current.Dispatcher.Invoke(() => {
                        Devices.Add(new DeviceViewModel(device.Id));
                    });
                }
            }).ContinueWith(_ => {
                IsRefreshing = false;
            });
        }

        void OnDeselect(DeviceViewModel device) {
            device.IsSelected = false;
        }

        void OnSelect(DeviceViewModel device) {
            device.IsSelected = true;
        }

        void OnCancel(object arg) {
            Close(false);
        }

        void OnApply(object arg) {
            Close(true);
        }
    }
}
