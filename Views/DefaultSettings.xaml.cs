using System.Windows;

namespace YankeeShower.Views
{
    /// <summary>
    /// Interaction logic for DefaultSettings.xaml
    /// </summary>
    public partial class DefaultSettings : Window
    {       

        public DefaultSettings()
        {
           
            InitializeComponent();
            DataContext = new ViewModels.DefaultSettingsViewModel();
            

        }

        

    }
}
