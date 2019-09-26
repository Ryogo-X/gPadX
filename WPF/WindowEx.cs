using System.Windows;

namespace gPadX.WPF {
    public class WindowEx : Window {
        public WindowEx() : base() {
            DataContextChanged += OnDataContextChanged;
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is ICanRequestClose oldVM) {
                oldVM.CloseRequest -= OnCloseRequested;
            }

            if (e.NewValue is ICanRequestClose newVM) {
                newVM.CloseRequest += OnCloseRequested;
            }
        }

        void OnCloseRequested(bool? value) {
            DialogResult = value;
            Close();
        }
    }
}
