using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace YankeeShower
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            if (!ConfigFileManager.GetConfigSettings())
            {
                MessageBox.Show("Missing or damaged configuration file", "Configuration File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
           
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An error has occurred " + e.Exception.TargetSite, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
            Shutdown(0);
        }


    }
}
