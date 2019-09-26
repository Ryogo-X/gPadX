using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gPadX.WPF {
    public abstract class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T newValue = default, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, newValue)) { return false; }

            field = newValue;

            Notify(propertyName);

            return true;
        }

        protected void Notify([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
