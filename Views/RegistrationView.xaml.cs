using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace YankeeShower.Views
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Window
    {
        public RegistrationView()
        {
            InitializeComponent();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                //DateTime _currentday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //string _decryptedcode = YankeeShower.Encrypt.DecryptString(registrationcode.Text, Environment.UserName);
                //if (!string.IsNullOrEmpty(_decryptedcode))
                //{
                //    ////extract expiry date and user name
                //    int _indx = _decryptedcode.LastIndexOf("[");
                //    if (_indx > -1)
                //    {
                //        ConfigFileManager.ExpiryDate = _decryptedcode.Substring(0, _indx);
                //        ConfigFileManager.User = _decryptedcode.Substring(_indx + 1, _decryptedcode.Length - 2 - _indx);
                //        if (Convert.ToDateTime(ConfigFileManager.ExpiryDate) >= _currentday)
                //        {
                //            if (ConfigFileManager.User == Environment.UserName)
                //            {
                //                ConfigFileManager.UpdateAppSetting("RegistrationType", "Licensed");
                //                ConfigFileManager.UpdateAppSetting("ExpiryDate", YankeeShower.Encrypt.EncryptString(ConfigFileManager.ExpiryDate + "[" + Environment.UserName + "]", Environment.UserName));

                //                Application.Current.Resources["Licence"] = "Licensed";
                //                Application.Current.Resources["ExpiryDate"] = ConfigFileManager.ExpiryDate.ToString();
                                
                //                this.DialogResult = (bool)true;
                //          //      this.Close();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show("Invalid registration code", "Registration Code Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //        e.Handled = true;
                //        this.DialogResult = false;
                //       // this.Close();
                //    }
                //}
                //else
                //{
                //    MessageBox.Show("Invalid registration code", "Registration Code Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    e.Handled = true;
                //    this.DialogResult = false;
                //   // this.Close();
                //}
            }
            catch 
            {
                e.Handled = true;
                MessageBox.Show("Registration code error", "Registration Code Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = false;                
               // this.Close();
            }           

        }
    }
}
