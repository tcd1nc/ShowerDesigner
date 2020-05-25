using System.Diagnostics;
using System.Reflection;
using System.Windows;


namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
            //Use File Version in Application | Assembly Information            
            version.Text = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

            licensee.Text = ConfigFileManager.ReadJsonValue("Licensee").ToString();

            if (Application.Current.Resources[Constants.eulaccepted].ToString() == "true")
            {
                closebtn.Visibility = Visibility.Visible;
            }
            else
            {
                decline.Visibility = Visibility.Visible;
                accept.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Decline_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {            
            Application.Current.Resources[Constants.eulaccepted] = "true";
            ConfigFileManager.WriteJsonValue("EulaAccepted", "true");
            Close();
        }
    }
}
