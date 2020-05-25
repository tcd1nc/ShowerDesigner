using System.Windows;

namespace YankeeShower.Views
{
    /// <summary>
    /// Interaction logic for NozzleSettings.xaml
    /// </summary>
    public partial class NozzleSettings : Window
    {
        public NozzleSettings(ViewModels.ShowerVM vm)
        {
            InitializeComponent();
            DataContext = new ViewModels.NozzleSettingsVM(vm);
        }
    }
}
