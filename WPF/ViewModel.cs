using System;

namespace gPadX.WPF {
    class ViewModel : ViewModelBase, ICanRequestClose {
        public event Action<bool?> CloseRequest;

        protected void Close(bool? value) {
            CloseRequest?.Invoke(value);
        }
    }
}
