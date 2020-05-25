using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for ShowerBoom.xaml
    /// </summary>
    public partial class ShowerBoom : UserControl, INotifyPropertyChanged
    {
        public ShowerBoom()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public static readonly DependencyProperty ShowerWidthProperty = DependencyProperty.Register("ShowerWidth", typeof(double), typeof(ShowerBoom),
                  new FrameworkPropertyMetadata(0.0));

        public double ShowerWidth
        {
            get { return (double)GetValue(ShowerWidthProperty); }
            set { SetValue(ShowerWidthProperty, value); }
        }
    }

}
