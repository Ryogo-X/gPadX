using System.Windows;
using gPadX.Utility;

namespace gPadX {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata {
                DefaultValue = Current.FindResource(typeof(Window))
            });
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            ConfigManager.Save();
        }
    }
}
