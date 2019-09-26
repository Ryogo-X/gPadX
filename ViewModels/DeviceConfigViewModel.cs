using System.Windows.Input;
using gPadX.Models;
using gPadX.Utility;
using gPadX.WPF;

namespace gPadX.ViewModels {
    class DeviceConfigViewModel : ViewModel {
        readonly DeviceConfigModel config;

        public string Alias { get; set; }
        public bool MapDPadToLS { get; set; }
        public int LSDeadZone { get; set; }
        public int RSDeadZone { get; set; }

        public ICommand CancelCommand { get; }
        public ICommand ApplyCommand { get; }

        public DeviceConfigViewModel(DeviceConfigModel config) {
            this.config = config;
            CancelCommand = new RelayCommand(OnCancel);
            ApplyCommand = new RelayCommand(OnApply);

            Update();
        }

        void Update() {
            Alias = config.Alias;
            MapDPadToLS = config.MapDPadToLS;
            LSDeadZone = config.LSDeadZone;
            RSDeadZone = config.RSDeadZone;
        }

        void OnCancel(object arg) {
            Close(false);
        }

        void OnApply(object arg) {
            config.Alias = Alias;
            config.MapDPadToLS = MapDPadToLS;
            config.LSDeadZone = LSDeadZone;
            config.RSDeadZone = RSDeadZone;
            ConfigManager.Save();

            Close(true);
        }
    }
}
