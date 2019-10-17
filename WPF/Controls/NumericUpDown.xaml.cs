using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace gPadX.WPF.Controls {
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl {
        public static readonly DependencyProperty IncrementValueProperty = DependencyProperty.Register(nameof(IncrementValue), typeof(int), typeof(NumericUpDown), new PropertyMetadata(1));

        public int IncrementValue {
            get { return (int)GetValue(IncrementValueProperty); }
            set { SetValue(IncrementValueProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnValueChanged));

        public int Minimum {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnValueChanged));

        public int Maximum {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnValueChanged, OnCoerceValue));

        public int Value {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty UpCommandProperty = DependencyProperty.Register(nameof(UpCommand), typeof(ICommand), typeof(NumericUpDown));

        public ICommand UpCommand {
            get { return (ICommand)GetValue(UpCommandProperty); }
            set { SetValue(UpCommandProperty, value); }
        }

        public static readonly DependencyProperty DownCommandProperty = DependencyProperty.Register(nameof(DownCommand), typeof(ICommand), typeof(NumericUpDown));

        public ICommand DownCommand {
            get { return (ICommand)GetValue(DownCommandProperty); }
            set { SetValue(DownCommandProperty, value); }
        }

        public NumericUpDown() {
            InitializeComponent();
            UpCommand = new RelayCommand(OnUp);
            DownCommand = new RelayCommand(OnDown);
            OnValueChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, Value, Value));
        }

        void OnUp(object arg) {
            Value++;
        }

        void OnDown(object arg) {
            Value--;
        }

        static void OnValueChanged(DependencyObject @object, DependencyPropertyChangedEventArgs args) {
            var sender = (NumericUpDown)@object;
            var value = (int)@object.GetValue(ValueProperty);

            sender.btnDown.IsEnabled = (value > (int)@object.GetValue(MinimumProperty));
            sender.btnUp.IsEnabled = (value < (int)@object.GetValue(MaximumProperty));
        }

        static object OnCoerceValue(DependencyObject @object, object value) {
            var numberValue = (int)value;
            if (numberValue < (int)@object.GetValue(MinimumProperty)) {
                return (int)@object.GetValue(MinimumProperty);
            } else if (numberValue > (int)@object.GetValue(MaximumProperty)) {
                return @object.GetValue(MaximumProperty);
            }

            return value;
        }

        void OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !int.TryParse(e.Text, out var number);
        }
    }
}